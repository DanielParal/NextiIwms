using Nexticz.Module.Mmo.Settings.Contracts.HistoryEvents;
using Nexticz.Module.Mmo.Settings.Domain;

namespace Nexticz.Module.Mmo.Settings.Presentation.HistoryEvents;

internal class HistoryEventResponseFactory
{
    public static HistoryEventResponse Create(HistoryEvent historyEvent)
    {
        return new HistoryEventResponse(
            historyEvent.Id,
            historyEvent.StreamId,
            historyEvent.Version,
            historyEvent.EventType,
            historyEvent.ExecutedAt,
            historyEvent.UserName,
            historyEvent.CorrelationId,
            historyEvent.Data);
    }
}