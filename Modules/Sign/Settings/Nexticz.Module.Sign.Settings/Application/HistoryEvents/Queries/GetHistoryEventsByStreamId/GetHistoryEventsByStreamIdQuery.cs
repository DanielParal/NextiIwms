using MediatR;
using Nexticz.Module.Sign.SharedKernel.DomainCore;
using Nexticz.Lib.Shared.DevExtreme;

namespace Nexticz.Module.Sign.Settings.Application.HistoryEvents.Queries.GetHistoryEventsByStreamId;

internal record GetHistoryEventsByStreamIdQuery(Guid StreamId, HistoryEventsFilteringParams FilteringParams) : IRequest<FilteredResult<HistoryEvent>>;