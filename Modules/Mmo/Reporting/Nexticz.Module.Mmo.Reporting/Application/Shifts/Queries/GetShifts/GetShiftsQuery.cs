using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;


namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShifts;

internal record GetShiftsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Shift>>;