using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Notifications.Presentation.Hubs;

internal static class HubEndpointsExtensions
{
    public static IEndpointRouteBuilder MapHubEndpointsExtensions(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapSignalRHubEndpoint();
    }
}