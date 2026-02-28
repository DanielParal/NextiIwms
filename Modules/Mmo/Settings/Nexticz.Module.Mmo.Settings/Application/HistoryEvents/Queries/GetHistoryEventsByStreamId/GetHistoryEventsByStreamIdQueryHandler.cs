using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain;


namespace Nexticz.Module.Mmo.Settings.Application.HistoryEvents.Queries.GetHistoryEventsByStreamId;

internal class GetHistoryEventsByStreamIdQueryHandler(ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository)
    : IRequestHandler<GetHistoryEventsByStreamIdQuery, FilteredResult<HistoryEvent>>
{
    public async Task<FilteredResult<HistoryEvent>> Handle(GetHistoryEventsByStreamIdQuery request, CancellationToken cancellationToken)
    {
        var events = await readOnlyEventStoreRepository.GetFilteredEventsByStreamIdAsync(request.StreamId, request.FilteringParams, cancellationToken);

        return events.MapDataFromTInToTOut(x => new HistoryEvent(x));
    }
}