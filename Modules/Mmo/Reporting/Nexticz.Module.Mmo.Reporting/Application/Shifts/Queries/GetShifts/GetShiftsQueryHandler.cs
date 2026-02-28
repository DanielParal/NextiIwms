using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;


namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShifts;

internal class GetShiftsQueryHandler(
    IReportingReadOnlyEventStoreRepository reportingReadOnlyRepository) 
    : IRequestHandler<GetShiftsQuery, FilteredResult<Shift>>
{
    public async Task<FilteredResult<Shift>> Handle(GetShiftsQuery request, CancellationToken cancellationToken)
    {
        var filteredShifts = await reportingReadOnlyRepository.GetFilteredAsync<Shift>(request.FilteringParams, cancellationToken);
        filteredShifts.Data = filteredShifts.Data.OrderByDescending(x => x.Schedule.Start).ToList();
        return filteredShifts;
    }
}