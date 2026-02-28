using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Washing.Presentation.WashingMachineSoses;

internal static class WashingMachineSosEndpointsExtensions
{
    public static IEndpointRouteBuilder MapWashingMachineSosEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCallSosEndpoint()
            .MapResolveSosEndpoint();
    }
}