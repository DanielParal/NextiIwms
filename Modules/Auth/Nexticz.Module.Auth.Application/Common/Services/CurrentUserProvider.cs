using Microsoft.AspNetCore.Http;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Lib.Shared.UserProviders;

namespace Nexticz.Module.Auth.Application.Common.Services;

public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    public CurrentUser GetCurrentUser()
    {
        if (httpContextAccessor.HttpContext is null)
        {
            return new CurrentUser
            {
                Id = Guid.Empty, UserName = string.Empty, Email = "cron", Roles = [], Permissions = [], UserDeviceInfo = "cron",
                XApiLanguage = "cs"
            };
        }

        var id = GetClaimValues(StringHelper.Claim.Type.MagicId)
            .Select(Guid.Parse)
            .FirstOrDefault();

        var email = GetClaimValues(StringHelper.Claim.Type.MagicEmail).FirstOrDefault() ?? "";
        var userName = GetClaimValues(StringHelper.Claim.Type.MagicUniqueName).FirstOrDefault() ?? "";
        var roles = GetClaimValues(StringHelper.Claim.Type.MagicRoles);
        var permissions = GetClaimValues(StringHelper.Claim.Type.MagicPermissions);

        var userAgent = httpContextAccessor.HttpContext.Request.Headers.UserAgent.FirstOrDefault();
        var ipAddresses = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
        var userDeviceInfo = ipAddresses + "||" + userAgent;

        var xApiLanguage = httpContextAccessor.HttpContext.Request.Headers[StringHelper.Header.XApiLanguage]
            .FirstOrDefault() ?? "Cs";

        return new CurrentUser
        {
            Id = id, UserName = userName, Email = email, Roles = roles, Permissions = permissions, UserDeviceInfo = userDeviceInfo,
            XApiLanguage = xApiLanguage
        };
    }

    private IReadOnlyList<string> GetClaimValues(string claimType)
    {
        return httpContextAccessor.HttpContext!.User.Claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)
            .ToList();
    }
}