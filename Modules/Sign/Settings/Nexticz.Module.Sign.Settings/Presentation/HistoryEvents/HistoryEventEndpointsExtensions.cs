using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.Settings.Presentation.HistoryEvents;

internal static class HistoryEventEndpointsExtensions
{
    public static IEndpointRouteBuilder MapHistoryEventsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetHistoryEventsByStreamId();
    }
}