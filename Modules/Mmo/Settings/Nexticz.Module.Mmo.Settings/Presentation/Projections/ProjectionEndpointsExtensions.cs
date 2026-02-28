using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.Projections;

internal static class ProjectionEndpointsExtensions
{
    public static IEndpointRouteBuilder MapProjectionEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapRebuildProjectionEndpoint();
    }
}