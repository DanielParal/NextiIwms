using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Washing.Presentation.LastEnteredWorkerOnLines;

internal static class LastEnteredWorkerOnLineEndpointsExtensions
{
    public static IEndpointRouteBuilder MapLastEnteredWorkerOnLineEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapEnterLineEndpoint()
            .MapLeaveLineEndpoint();
    }
}