using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Auth.Contracts.Authentications;
using Nexticz.Module.Auth.Domain.AppRoles;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Module.Auth.Application.Authentications.Common;
using Nexticz.Module.Auth.Application.Common.Interfaces;

namespace Nexticz.Module.Auth.Application.Authentications.Commands.Register;

public class RegisterCommandHandler(
    UserManager<AppUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator,
    IAppUserService appUserService)
    : IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResponse>>
{
    public async Task<ErrorOr<AuthenticationResponse>> Handle(RegisterCommand command,
        CancellationToken cancellationToken)
    {
        if (await userManager.Users
                .AnyAsync(x => x.UserName == command.RegisterRequest.Username || x.Email == command.RegisterRequest.Email, cancellationToken))
            return AuthenticationErrors.UserExist;

        var newAppUser = new AppUser(command.RegisterRequest.Username, command.RegisterRequest.Email);

        var createdUserResult = await userManager.CreateAsync(newAppUser, command.RegisterRequest.Password);

        if (!createdUserResult.Succeeded) return appUserService.CreateUserResultErrors(createdUserResult.Errors);
        
        // var createdAppUser = await userManager.Users
        //     .Include(x => x.AppUserRoles)!
        //     .ThenInclude(x => x.AppRole)
        //     .Include(x => x.AppUserClaims)
        //     .SingleOrDefaultAsync(x => x.UserName == request.RegisterRequest.Username, cancellationToken);

        // createdAppUser.ThrowIfNull();
        
        var refreshToken = jwtTokenGenerator.GenerateRefreshToken();

        await appUserService.AddRefreshTokenToUser(newAppUser, refreshToken, cancellationToken);

        var addedDefaultValue = await appUserService.AddDefaultUserLoginsRolesAndPermissionsToUser(newAppUser);

        if (addedDefaultValue.IsError) return addedDefaultValue.Errors;

        return new AuthenticationResponse
        {
            Id = newAppUser.Id,
            Username = newAppUser.UserName!,
            Email = newAppUser.Email!,
            Company = newAppUser.Company,
            Firsname = newAppUser.Firstname,
            Lastname = newAppUser.Lastname,
            PhoneNumber = newAppUser.PhoneNumber,
            AccessToken = jwtTokenGenerator.GenerateAccessToken(newAppUser),
            RefreshToken = refreshToken
        };
    }
}

// todo enable in addIdentity RequireConfirmedAccount and send a implement confirmation
// var confirmationToken =
//     HttpUtility.UrlEncode(await userManager.GenerateEmailConfirmationTokenAsync(createdAppUser!));
// var result = SendConfirmationEmail(createdAppUser, confirmationToken);
//
// if (result.IsError)
// {
//     return result.Errors;
// }