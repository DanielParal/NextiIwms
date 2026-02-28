using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Reporting.Presentation.LineItems;

internal static class LineItemEndpointsExtensions
{
    public static IEndpointRouteBuilder MapLineItemsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapChangeCommentEndpoint()
            .MapChangeItemEndpoint()
            .MapAddItemsEndpoint();
    }
}