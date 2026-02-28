using System.Security.Claims;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Module.Auth.Domain.AppRoles;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Auth.Application.Authentications.Common;
using Nexticz.Module.Auth.Application.Common.Interfaces;
using Nexticz.Module.Auth.Application.MasstransitPublishers;

namespace Nexticz.Module.Auth.Application.Accounts.Commands.CreateAccount;

public class CreateAccountCommandHandler(
    UserManager<AppUser> userManager,
    RoleManager<AppRole> roleManager,
    IAppUserService appUserService,
    IAuthPublisher authPublisher,
    ILogger<CreateAccountCommandHandler> logger)
    : IRequestHandler<CreateAccountCommand, ErrorOr<AppUser>>
{
    public async Task<ErrorOr<AppUser>> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        if (await userManager.Users
                .AnyAsync(
                    x => x.UserName == command.CreateAccountRequest.Username ||
                         x.Email == command.CreateAccountRequest.Email, cancellationToken))
            return AuthenticationErrors.UserExist;

        var newAppUser = new AppUser
        (
            command.CreateAccountRequest.Username,
            command.CreateAccountRequest.Email,
            command.CreateAccountRequest.PhoneNumber,
            command.CreateAccountRequest.Firstname,
            command.CreateAccountRequest.Lastname,
            command.CreateAccountRequest.Company,
            command.CreateAccountRequest.BlockedFrom
        );
        
        var createdUserResult = string.IsNullOrWhiteSpace(command.CreateAccountRequest.Password)
            ? await userManager.CreateAsync(newAppUser)
            : await userManager.CreateAsync(newAppUser, command.CreateAccountRequest.Password!);

        if (!createdUserResult.Succeeded) return appUserService.CreateUserResultErrors(createdUserResult.Errors);

        foreach (var role in command.CreateAccountRequest.Roles)
        {
            var roleExist = await roleManager.RoleExistsAsync(role);

            if (!roleExist) await roleManager.CreateAsync(new AppRole { Name = role });
        }

        if (command.CreateAccountRequest.Roles.Length == 0)
        {
            var addedDefaultValue = await appUserService.AddDefaultUserLoginsRolesAndPermissionsToUser(newAppUser);
            if (addedDefaultValue.IsError) return addedDefaultValue.Errors;
        }

        await userManager.AddToRolesAsync(newAppUser, command.CreateAccountRequest.Roles);

        logger.LogInformation("AUTH - user created, id: {Id}, username: {username}", 
            newAppUser.Id, newAppUser.UserName);
        
        await authPublisher.PublishUserChangedMessageAsync(
            newAppUser.Id,
            command.CreateAccountRequest.Username,
            newAppUser.Firstname,
            newAppUser.Lastname,
            command.CreateAccountRequest.Roles,
            command.CreateAccountRequest.Permissions,
            UserChangeTypeContract.Created,
            cancellationToken);
        
        if (command.CreateAccountRequest.Permissions.Length == 0)
            return newAppUser;
        
        List<Claim> newUserClaims = [];
        newUserClaims.AddRange(command.CreateAccountRequest.Permissions!.ToList()
            .Select(x => new Claim(StringHelper.Claim.Type.MagicPermissions, x)));

        await userManager.AddClaimsAsync(newAppUser, newUserClaims);
        
        return newAppUser;
    }
}