using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Interfaces;

internal interface IWashingMachineSpeedReadOnlyRepository
{
    Task<WashingMachineSpeed?> GetCurrentWashingMachineSpeedByCodeAsync(string code, CancellationToken cancellationToken);
    Task<IReadOnlyList<WashingMachineSpeed>> GetWashingMachineSpeedsByTimeRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken);
}