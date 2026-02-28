using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Domain;


namespace Nexticz.Module.Mmo.Settings.Application.HistoryEvents.Queries.GetHistoryEventsByStreamId;

internal record GetHistoryEventsByStreamIdQuery(Guid StreamId, HistoryEventsFilteringParams FilteringParams) : IRequest<FilteredResult<HistoryEvent>>;