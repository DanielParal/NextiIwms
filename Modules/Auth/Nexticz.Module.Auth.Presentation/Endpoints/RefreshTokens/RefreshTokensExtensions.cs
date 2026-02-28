using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Auth.Presentation.Endpoints.RefreshTokens;

public static class RefreshTokensExtensions
{
    public static IEndpointRouteBuilder MapRefreshTokensEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapDeleteRefreshTokenById()
            .MapGetRefreshTokensByUserId();
    }
}