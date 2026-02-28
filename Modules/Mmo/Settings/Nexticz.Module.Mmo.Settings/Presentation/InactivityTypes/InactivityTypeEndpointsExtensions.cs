using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.InactivityTypes;

internal static class InactivityTypeEndpointsExtensions
{
    public static IEndpointRouteBuilder MapInactivityTypesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateInactivityTypeEndpoint()
            .MapUpdateInactivityTypeEndpoint()
            .MapDeleteInactivityTypeEndpoint()
            .MapGetInactivityTypeByIdEndpoint();
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionInactivityTypesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetInactivityTypesEndpoint();
    }
}