using ErrorOr;
using Nexticz.Module.Auth.Domain.ApiKeyAggregate;

namespace Nexticz.Module.Auth.Infrastructure.Identity;

internal static class AppUserApiKeyMapper
{
    public static ErrorOr<ApiKey> ToDomain(AppUserApiKey appUserApiKey)
    {
        return ApiKey.CreateFrom(appUserApiKey.Id, appUserApiKey.UserId, appUserApiKey.Value, appUserApiKey.Description,
            appUserApiKey.Created, appUserApiKey.LastActivity, appUserApiKey.Expiration);
    }
}