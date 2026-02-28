using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Auth.Presentation.Endpoints.Me;

public static class MeExtensions
{
    public static IEndpointRouteBuilder MapMeEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetMe();
    }
}