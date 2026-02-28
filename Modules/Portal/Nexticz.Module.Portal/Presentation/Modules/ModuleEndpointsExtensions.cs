using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Portal.Presentation.Modules;

internal static class ModuleEndpointsExtensions
{
    public static IEndpointRouteBuilder MapModulesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateModuleEndpoint()
            .MapUpdateModuleEndpoint()
            .MapDeleteModuleEndpoint()
            .MapGetModuleByIdEndpoint()
            .MapChangeModuleOrderEndpoint();
    }
    
    public static IEndpointRouteBuilder MapModulesAnyPermissionEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetModulesEndpoint();
    }
}