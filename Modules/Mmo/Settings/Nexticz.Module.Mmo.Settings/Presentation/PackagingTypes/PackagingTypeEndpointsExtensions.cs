using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.PackagingTypes;

internal static class PackagingTypeEndpointsExtensions
{
    public static IEndpointRouteBuilder MapPackagingTypesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreatePackagingType()
            .MapUpdatePackagingType()
            .MapDeletePackagingType()
            .MapGetPackagingTypeByCode();
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionPackagingTypesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetPackagingTypes();
    }
}