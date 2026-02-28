using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Washing.Presentation.WashingStates;

internal static class WashingStatesEndpointsExtensions
{
    public static IEndpointRouteBuilder MapWashingStatesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetWashingStateEndpoint();
    }
}