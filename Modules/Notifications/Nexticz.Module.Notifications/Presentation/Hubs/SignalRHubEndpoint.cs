using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Notifications.Application.SignalRNotifications;

namespace Nexticz.Module.Notifications.Presentation.Hubs;

internal static class SignalRHubEndpoint
{
    public static IEndpointRouteBuilder MapSignalRHubEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapHub<SignalRHub>(NotificationsEndpoints.HubEndpoints.SignalR);
        
        return builder;
    }
}