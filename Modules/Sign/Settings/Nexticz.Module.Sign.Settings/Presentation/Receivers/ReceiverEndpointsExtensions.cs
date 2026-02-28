using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.Settings.Presentation.Receivers;

internal static class ReceiverEndpointsExtensions
{
    public static IEndpointRouteBuilder MapReceiversEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateReceiverEndpoint()
            .MapGetReceiverByCodeAndPartnerCodeEndpoint()
            .MapGetReceiversEndpoint()
            .MapUpdateReceiverEndpoint()
            .MapDeleteReceiverEndpoint();
    }
}