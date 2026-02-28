using MediatR;
using Nexticz.Module.Sign.SharedKernel.DomainCore;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Application.Interfaces;

namespace Nexticz.Module.Sign.Settings.Application.HistoryEvents.Queries.GetHistoryEventsByStreamId;

internal class GetHistoryEventsByStreamIdQueryHandler(ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository)
    : IRequestHandler<GetHistoryEventsByStreamIdQuery, FilteredResult<HistoryEvent>>
{
    public async Task<FilteredResult<HistoryEvent>> Handle(GetHistoryEventsByStreamIdQuery request, CancellationToken cancellationToken)
    {
        var events = await readOnlyEventStoreRepository.GetFilteredEventsByStreamIdAsync(request.StreamId, request.FilteringParams, cancellationToken);

        return events.MapDataFromTInToTOut(x => new HistoryEvent(x));
    }
}