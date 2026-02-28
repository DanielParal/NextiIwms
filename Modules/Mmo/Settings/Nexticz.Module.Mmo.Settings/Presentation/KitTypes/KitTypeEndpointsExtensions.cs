using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.KitTypes;

internal static class KitTypeEndpointsExtensions
{
    public static IEndpointRouteBuilder MapKitTypesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateKitType()
            .MapUpdateKitType()
            .MapDeleteKitType()
            .MapGetKitTypeByCode();
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionKitTypesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetKitTypes();
    }
}