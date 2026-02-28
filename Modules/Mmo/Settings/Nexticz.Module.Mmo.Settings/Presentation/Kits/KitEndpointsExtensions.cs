using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.Kits;

internal static class KitEndpointsExtensions
{
    public static IEndpointRouteBuilder MapKitsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetKitById()
            .MapCreateKit()
            .MapUpdateKit()
            .MapDeleteKit()
            .MapUploadKitInstructionEndpoint()
            .MapDeleteKitInstructionEndpoint()
            .MapGetKitSpecialInformationsEndpoint();
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionKitsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetKits()
            .MapGetKitInstructionFileEndpoint();
    }
}