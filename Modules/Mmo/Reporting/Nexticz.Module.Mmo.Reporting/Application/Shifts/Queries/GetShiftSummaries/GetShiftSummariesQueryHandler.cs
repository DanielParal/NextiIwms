using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Models;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftSummaryById;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;


namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftSummaries;

internal class GetShiftSummariesQueryHandler(
    IReportingReadOnlyEventStoreRepository reportingReadOnlyRepository,
    ISender sender)  : IRequestHandler<GetShiftSummariesQuery, FilteredResult<ShiftSummary>>
{
    public async Task<FilteredResult<ShiftSummary>> Handle(GetShiftSummariesQuery request, CancellationToken cancellationToken)
    {
        var filteredShifts = await reportingReadOnlyRepository.GetFilteredAsync<Shift>(request.FilteringParams, cancellationToken);
        filteredShifts.Data = filteredShifts.Data.OrderByDescending(x => x.Schedule.Start).ToList();
        
        var shiftSummaries = new List<ShiftSummary>();
        foreach (var shift in filteredShifts.Data)
        {
            var shiftSummary = await sender.Send(new GetShiftSummaryByIdQuery(shift), cancellationToken);
            shiftSummaries.Add(shiftSummary);
        }

        return new FilteredResult<ShiftSummary>
        {
            Data = shiftSummaries,
            TotalCount = filteredShifts.TotalCount,
            GroupCount = filteredShifts.GroupCount,
            Summary = filteredShifts.Summary,
        };
    }
}