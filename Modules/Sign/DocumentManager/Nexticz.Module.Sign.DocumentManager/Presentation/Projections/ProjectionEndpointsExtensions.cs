using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.Projections;

internal static class ProjectionEndpointsExtensions
{
    public static IEndpointRouteBuilder MapProjectionEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapRebuildProjectionEndpoint();
    }
}