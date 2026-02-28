using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Washing.Presentation.Simulations;

internal static class SimulationEndpointsExtensions
{
    public static IEndpointRouteBuilder MapSimulationEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapStartSimulationEndpoint()
            .MapStopSimulationEndpoint();
    }
}