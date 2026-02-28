using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.Settings.Presentation.DepositorGroups;

internal static class DepositorGroupEndpointsExtensions
{
    public static IEndpointRouteBuilder MapDepositorGroupsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateDepositorGroupEndpoint()
            .MapGetDepositorGroupByCodeEndpoint()
            .MapGetDepositorGroupsEndpoint()
            .MapUpdateDepositorGroupEndpoint()
            .MapDeleteDepositorGroupEndpoint();
    }
}