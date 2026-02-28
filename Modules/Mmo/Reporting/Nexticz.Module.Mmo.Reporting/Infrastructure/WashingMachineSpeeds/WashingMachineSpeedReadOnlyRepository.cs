using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Reporting.Infrastructure.WashingMachineSpeeds;

internal class WashingMachineSpeedReadOnlyRepository(
    IReportingReadOnlyEventStoreRepository reportingReadOnlyEventStoreRepository)
    : IWashingMachineSpeedReadOnlyRepository
{
    public async Task<WashingMachineSpeed?> GetCurrentWashingMachineSpeedByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await reportingReadOnlyEventStoreRepository.GetFirstByConditionAsync<WashingMachineSpeed>(
            x => x.Code == code && x.DateEnded == null, cancellationToken);
    }

    public async Task<IReadOnlyList<WashingMachineSpeed>> GetWashingMachineSpeedsByTimeRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken)
    {
        return await reportingReadOnlyEventStoreRepository.GetAllByConditionAsync<WashingMachineSpeed>(speed => 
                (speed.DateStarted >= startDate && speed.DateStarted <= endDate) || 
                (speed.DateEnded >= startDate && speed.DateEnded <= endDate) || 
                (speed.DateStarted <= startDate && speed.DateEnded == null), 
            cancellationToken);
    }
}