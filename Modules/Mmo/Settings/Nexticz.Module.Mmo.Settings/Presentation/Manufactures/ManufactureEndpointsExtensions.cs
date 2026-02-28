using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.Manufactures;

internal static class ManufactureEndpointsExtensions
{
    public static IEndpointRouteBuilder MapManufacturesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateManufacture()
            .MapUpdateManufacture()
            .MapDeleteManufacture()
            .MapGetManufactureByCode();
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionManufacturesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetManufactures();
    }
}