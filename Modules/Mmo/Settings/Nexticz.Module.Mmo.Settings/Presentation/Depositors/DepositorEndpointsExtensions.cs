using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.Depositors;

internal static class DepositorEndpointsExtensions
{
    public static IEndpointRouteBuilder MapDepositorsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateDepositor()
            .MapUpdateDepositor()
            .MapDeleteDepositor()
            .MapGetDepositorByCode();
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionDepositorsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetDepositors();
    }
}