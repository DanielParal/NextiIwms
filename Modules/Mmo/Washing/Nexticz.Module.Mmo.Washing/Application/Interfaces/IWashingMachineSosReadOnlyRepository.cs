using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Interfaces;

internal interface IWashingMachineSosReadOnlyRepository
{
    Task<WashingMachineSos?> GetWashingMachineSosByCodeAsync(string washingMachineCode, CancellationToken cancellationToken);
}