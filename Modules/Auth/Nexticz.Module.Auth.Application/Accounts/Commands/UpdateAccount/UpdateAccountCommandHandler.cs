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
using Nexticz.Module.Auth.Application.Accounts.Queries.GetAccountById;
using Nexticz.Module.Auth.Application.Authentications.Common;
using Nexticz.Module.Auth.Application.MasstransitPublishers;

namespace Nexticz.Module.Auth.Application.Accounts.Commands.UpdateAccount;

public class UpdateAccountCommandHandler(
    UserManager<AppUser> userManager,
    RoleManager<AppRole> roleManager,
    IPasswordHasher<AppUser> passwordHasher,
    ILogger<UpdateAccountCommandHandler> logger,
    ISender sender,
    IAuthPublisher authPublisher)
    : IRequestHandler<UpdateAccountCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateAccountCommand command, CancellationToken cancellationToken)
    {
        using var loggerScope = logger.BeginScope(new Dictionary<string, object>
        {
            [nameof(AppUser.UserName)] = command.Id
        });

        logger.LogInformation("AUTH - Consuming UpdateAccountCommand, id: {Id}, username: {username}", 
            command.Id, command.UpdateAccountRequest.Username);

        var appUser = await userManager.Users
            .Where(x => x.Id == command.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (appUser is null)
        {
            logger.LogError("AUTH - InvalidPassword");
            return AuthenticationErrors.InvalidPassword;
        }

        if (!string.IsNullOrWhiteSpace(command.UpdateAccountRequest.Password))
        {
            var validator = new PasswordValidator<AppUser>();
            var result = await validator.ValidateAsync(userManager, null!, command.UpdateAccountRequest.Password);
            if (!result.Succeeded) 
                return AppUserErrors.UpdateAppUserError;
            appUser.PasswordHash = passwordHasher.HashPassword(appUser, command.UpdateAccountRequest.Password);
        }

        appUser.Email = command.UpdateAccountRequest.Email;
        appUser.PhoneNumber = command.UpdateAccountRequest.PhoneNumber;
        appUser.Firstname = command.UpdateAccountRequest.Firstname;
        appUser.Lastname = command.UpdateAccountRequest.Lastname;
        appUser.Company = command.UpdateAccountRequest.Company;
        appUser.BlockedFrom = command.UpdateAccountRequest.BlockedFrom;

        var userInRoles = await userManager.GetRolesAsync(appUser);
        await userManager.RemoveFromRolesAsync(appUser, userInRoles);

        foreach (var role in command.UpdateAccountRequest.Roles)
        {
            var roleExist = await roleManager.RoleExistsAsync(role);

            if (!roleExist) await roleManager.CreateAsync(new AppRole { Name = role });
        }

        await userManager.AddToRolesAsync(appUser, command.UpdateAccountRequest.Roles);

        var userClaims = await userManager.GetClaimsAsync(appUser);

        await userManager.RemoveClaimsAsync(appUser, userClaims);

        List<Claim> newUserClaims = [];

        if (command.UpdateAccountRequest.Permissions.Length > 0)
        {
            newUserClaims.AddRange(command.UpdateAccountRequest.Permissions!.ToList()
                .Select(x => new Claim(StringHelper.Claim.Type.MagicPermissions, x)));

            await userManager.AddClaimsAsync(appUser, newUserClaims);
        }

        var updateResult = await userManager.UpdateAsync(appUser);

        if (!updateResult.Succeeded) 
            return AppUserErrors.UpdateAppUserError;

        logger.LogInformation("AUTH - user updated, id: {Id}, username: {username}", appUser.Id, appUser.UserName);

        var appUserToPublish = await sender.Send(new GetAccountByIdQuery(appUser.Id), cancellationToken);
                
        await authPublisher.PublishUserChangedMessageAsync(
            appUserToPublish.Value.Id,
            appUserToPublish.Value.Username,
            appUserToPublish.Value.Firstname,
            appUserToPublish.Value.Lastname,
            appUserToPublish.Value.Roles,
            appUserToPublish.Value.Permissions,
            UserChangeTypeContract.Updated,
            cancellationToken);
        
        
        return Result.Updated;
    }
}