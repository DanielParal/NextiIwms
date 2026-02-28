using Nexticz.Module.Auth.Domain.UserAggregate;

namespace Nexticz.Module.Auth.Infrastructure.Identity;

internal static class AppUserMapper
{
    public static User? ToDomain(AppUser? appUser)
    {
        if (appUser is null) 
            return null;

        var roles = appUser.AppUserRoles is null
            ? []
            : NonEmpty(appUser.AppUserRoles.Select(x => x.AppRole?.Name));

        var permissions = appUser.AppUserClaims is null
            ? []
            :  NonEmpty(appUser.AppUserClaims.Select(x => x.ClaimValue));

        var result = User.CreateFrom(
            appUser.UserName ?? string.Empty,
            appUser.Email ?? string.Empty,
            appUser.PhoneNumber,
            appUser.Firstname,
            appUser.Lastname,
            appUser.Company,
            roles,
            permissions,
            appUser.AppUserApiKeys.Select(x => x.Id).ToArray(),
            appUser.BlockedFrom,
            appUser.LastActivity);

        return result.IsError ? null : result.Value;
    }

    private static string[] NonEmpty(IEnumerable<string?> values) =>
        values
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .ToArray();

}