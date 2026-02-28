using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Models;


namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftSummaries;

internal record GetShiftSummariesQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<ShiftSummary>>;