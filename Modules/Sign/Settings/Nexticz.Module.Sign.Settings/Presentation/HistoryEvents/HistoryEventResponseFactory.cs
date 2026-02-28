using Nexticz.Module.Sign.Settings.Contracts.HistoryEvents;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Presentation.HistoryEvents;

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