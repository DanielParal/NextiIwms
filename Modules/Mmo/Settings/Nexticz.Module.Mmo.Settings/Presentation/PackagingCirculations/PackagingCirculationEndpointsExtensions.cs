using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.PackagingCirculations;

internal static class PackagingCirculationEndpointsExtensions
{
    public static IEndpointRouteBuilder MapPackagingCirculationsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreatePackagingCirculation()
            .MapUpdatePackagingCirculation()
            .MapDeletePackagingCirculation()
            .MapGetPackagingCirculationByCode();
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionPackagingCirculationsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetPackagingCirculations();
    }
}