using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.Settings.Presentation.DeliveryMethods;

internal static class DeliveryMethodEndpointsExtensions
{
    public static IEndpointRouteBuilder MapDeliveryMethodsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetDeliveryMethodsEndpoint()
            .MapGetDeliveryMethodByCodeEndpoint()
            .MapCreateDeliveryMethodEndpoint()
            .MapUpdateDeliveryMethodEndpoint()
            .MapDeleteDepositorGroupEndpoint();
    }
}