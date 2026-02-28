using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.Settings.Presentation.Seeds;

internal static class SeedEndpointsExtensions
{
    public static IEndpointRouteBuilder MapSeedsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateSeed();
    }
}