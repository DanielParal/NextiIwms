using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Infrastructure.Shifts;

internal class ShiftReadOnlyRepository(
    IReportingReadOnlyEventStoreRepository reportingReadOnlyEventStoreRepository) 
    : IShiftReadOnlyRepository
{
    public async Task<Shift?> GetShiftByStartDateAndEndDateAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken)
    {
        return await reportingReadOnlyEventStoreRepository.GetFirstByConditionAsync<Shift>(
            x => x.Schedule.Start == startDate && x.Schedule.End == endDate, cancellationToken);
    }

    public async Task<Shift?> GetLastShiftAsync(CancellationToken cancellationToken)
    {
        return await reportingReadOnlyEventStoreRepository.GetFirstByConditionAsync<Shift>(
            x => x.IsLast, cancellationToken);
    }

    public async Task<Shift?> GetNextToLastShiftAsync(CancellationToken cancellationToken)
    {
        return await reportingReadOnlyEventStoreRepository.GetFirstByConditionAsync<Shift>(
            x => x.IsNextToLast, cancellationToken);
    }
}