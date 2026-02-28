using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.Depositors;

public static class DepositorsExtensions
{
    public static IEndpointRouteBuilder MapDepositorsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateDepositor()
            .MapUpdateDepositor()
            .MapDeleteDepositor()
            .MapGetDepositorById()
            .MapGetDepositors();
    }
}