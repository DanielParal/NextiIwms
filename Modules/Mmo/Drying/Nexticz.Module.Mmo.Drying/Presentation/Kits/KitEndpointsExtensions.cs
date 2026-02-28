using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Drying.Presentation.Kits;

internal static class KitEndpointsExtensions
{
    public static IEndpointRouteBuilder MapKitEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapFinishKitEndpoint()
            .MapGetKitByCompletedKitsCountEndpoint()
            .MapTransferKitEndpoint();
    }
    
    public static IEndpointRouteBuilder MapGetAllKitsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetKitsEndpoint();
    }
}