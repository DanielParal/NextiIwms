using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.WashingMachines;

internal static class WashingMachineEndpointsExtensions
{
    public static IEndpointRouteBuilder MapWashingMachinesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateWashingMachine()
            .MapUpdateWashingMachine()
            .MapGetWashingMachineByCode();
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionWashingMachinesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetWashingMachines();
    }
}