using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Interfaces;

internal interface IWashingMachineSpeedReadOnlyRepository
{
    Task<WashingMachineSpeed?> GetWashingMachineSpeedByCodeAsync(string washingMachineCode, CancellationToken cancellationToken);
}