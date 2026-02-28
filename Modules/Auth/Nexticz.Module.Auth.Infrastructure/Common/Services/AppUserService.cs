using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Nexticz.Module.Auth.Application.Authentications.Common;
using Nexticz.Module.Auth.Application.Common.Interfaces;
using Nexticz.Module.Auth.Domain.AppRoles;
using Nexticz.Module.Auth.Domain.AppUserRefreshTokens;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Lib.Shared.UserProviders;

namespace Nexticz.Module.Auth.Infrastructure.Common.Services;

public class AppUserService(
    IUnitOfWork unitOfWork,
    UserManager<AppUser> userManager,
    RoleManager<AppRole> roleManager,
    ICurrentUserProvider currentUserProvider,
    IConfiguration configuration) : IAppUserService
{
    public async Task<ErrorOr<Success>> AddDefaultUserLoginsRolesAndPermissionsToUser(AppUser newAppUser)
    {
        var userLoginInfo = new UserLoginInfo(nameof(AuthenticationMethodType.UsenamePassword),
            Guid.NewGuid().ToString(), nameof(AuthenticationMethodType.UsenamePassword));

        await userManager.AddLoginAsync(newAppUser, userLoginInfo);

        var anonymousRole = roleManager.Roles.FirstOrDefault(x => x.Name == AppRoleType.Anonymous.ToString());

        if (anonymousRole is null) return AuthenticationErrors.RegistrationError;

        await userManager.AddToRoleAsync(newAppUser, anonymousRole.NormalizedName!);

        var anonymousPermissions = await roleManager.GetClaimsAsync(anonymousRole);

        await userManager.AddClaimsAsync(newAppUser, anonymousPermissions);

        return Result.Success;
    }

    public async Task AddRefreshTokenToUser(AppUser appUser, string refreshToken,
        CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var appUserRefreshToken = new AppUserRefreshToken
            { UserId = appUser.Id, Value = refreshToken, UserDeviceInfo = currentUser.UserDeviceInfo };

        await unitOfWork.AppUserRefreshTokensRepository.AddAppUserRefreshTokenAsync(appUserRefreshToken,
            cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
    }

    public List<Error> CreateUserResultErrors(IEnumerable<IdentityError> identityErrors)
    {
        var errors = new List<Error>();
        foreach (var identityError in identityErrors)
            errors.Add(identityError.Code switch
            {
                nameof(IdentityPasswordValidationerror.InvalidEmail) => AuthenticationErrors.InvalidEmail,
                nameof(IdentityPasswordValidationerror.DuplicateEmail) => AuthenticationErrors.DuplicateEmail,
                nameof(IdentityPasswordValidationerror.PasswordTooShort) => AuthenticationErrors.PasswordTooShort,
                nameof(IdentityPasswordValidationerror.PasswordRequiresNonAlphanumeric) => AuthenticationErrors
                    .PasswordRequiresNonAlphanumeric,
                nameof(IdentityPasswordValidationerror.PasswordRequiresDigit) => AuthenticationErrors
                    .PasswordRequiresDigit,
                nameof(IdentityPasswordValidationerror.PasswordRequiresUpper) => AuthenticationErrors
                    .PasswordRequiresUpper,
                nameof(IdentityPasswordValidationerror.PasswordRequiresLower) => AuthenticationErrors
                    .PasswordRequiresLower,
                nameof(IdentityPasswordValidationerror.PasswordRequiresUniqueChars) => AuthenticationErrors
                    .PasswordRequiresUniqueChars,
                _ => AuthenticationErrors.PasswordUnspecifiedError
            });

        return errors;
    }

    private enum IdentityPasswordValidationerror
    {
        InvalidEmail,
        DuplicateEmail,
        PasswordTooShort,
        PasswordRequiresNonAlphanumeric,
        PasswordRequiresDigit,
        PasswordRequiresUpper,
        PasswordRequiresLower,
        PasswordRequiresUniqueChars
    }
}