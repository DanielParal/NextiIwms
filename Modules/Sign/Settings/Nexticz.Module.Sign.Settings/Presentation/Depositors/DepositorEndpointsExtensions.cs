using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.Settings.Presentation.Depositors;

internal static class DepositorEndpointsExtensions
{
    public static IEndpointRouteBuilder MapDepositorsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateDepositorEndpoint()
            .MapGetDepositorByCodeEndpoint()
            .MapGetDepositorsEndpoint()
            .MapUpdateDepositorEndpoint()
            .MapDeleteDepositorEndpoint();
    }
}