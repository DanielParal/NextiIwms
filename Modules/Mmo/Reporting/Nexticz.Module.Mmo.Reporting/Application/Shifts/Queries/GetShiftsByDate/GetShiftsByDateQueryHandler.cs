using MediatR;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftsByDate;

internal class GetShiftsByDateQueryHandler(
    IReportingReadOnlyEventStoreRepository reportingReadOnlyEventStoreRepository,
    IClock clock) 
    : IRequestHandler<GetShiftsByDateQuery, Shift[]>
{
    public async Task<Shift[]> Handle(GetShiftsByDateQuery request, CancellationToken cancellationToken)
    {
        // tenant-local day start & end
        var localDayStart = request.SelectedDate.ToDateTime(TimeOnly.MinValue);
        var localDayEnd = request.SelectedDate.ToDateTime(TimeOnly.MaxValue);

        // convert to UTC DateTimeOffset
        var dayStartUtc = clock.ConvertTenantToUtcDateTime(localDayStart);
        var dayEndUtc = clock.ConvertTenantToUtcDateTime(localDayEnd);
        
        var shifts = await reportingReadOnlyEventStoreRepository.GetAllByConditionAsync<Shift>(
            x => (x.Schedule.Start >= dayStartUtc && x.Schedule.Start <= dayEndUtc)
                 || (x.Schedule.End >= dayStartUtc && x.Schedule.End <= dayEndUtc),
            cancellationToken);
        
        return shifts
            .OrderBy(x => x.Schedule.Start)
            .ToArray();
    } 
}