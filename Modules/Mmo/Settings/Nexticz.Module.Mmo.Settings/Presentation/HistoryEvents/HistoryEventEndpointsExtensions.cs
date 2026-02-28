using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.HistoryEvents;

internal static class HistoryEventEndpointsExtensions
{
    public static IEndpointRouteBuilder MapHistoryEventsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetHistoryEventsByStreamId();
    }
}