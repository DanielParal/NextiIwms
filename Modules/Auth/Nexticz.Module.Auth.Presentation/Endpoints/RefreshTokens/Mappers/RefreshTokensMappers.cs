using Nexticz.Module.Auth.Contracts.RefreshTokens;
using Nexticz.Module.Auth.Domain.AppUserRefreshTokens;

namespace Nexticz.Module.Auth.Presentation.Endpoints.RefreshTokens.Mappers;

public static class RefreshTokensMappers
{
    public static RefreshTokenResponse MapToRefreshTokenResponse(this AppUserRefreshToken appUserRefreshToken)
    {
        return new RefreshTokenResponse
        {
            Id = appUserRefreshToken.Id,
            UserId = appUserRefreshToken.UserId,
            Value = appUserRefreshToken.Value,
            UserDeviceInfo = appUserRefreshToken.UserDeviceInfo,
            Created = appUserRefreshToken.Created,
            Expiration = appUserRefreshToken.Expiration,
            LastActivity = appUserRefreshToken.LastActivity
        };
    }

    public static List<RefreshTokenResponse> MapToRefreshTokensResponse(
        this List<AppUserRefreshToken> appUserRefreshToken)
    {
        return appUserRefreshToken.Select(x => x.MapToRefreshTokenResponse()).ToList();
    }
}