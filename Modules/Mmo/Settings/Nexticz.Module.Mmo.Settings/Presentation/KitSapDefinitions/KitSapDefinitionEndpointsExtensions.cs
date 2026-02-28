using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.KitSapDefinitions;

internal static class KitSapDefinitionEndpointsExtensions
{
    public static IEndpointRouteBuilder MapKitSapDefinitionsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetKitSapDefinitionByCode()
            .MapCreateKitSapDefinition()
            .MapUpdateKitSapDefinition()
            .MapDeleteKitSapDefinition();
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionKitSapDefinitionsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetKitSapDefinitions();
    }
}