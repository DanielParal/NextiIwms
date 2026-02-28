using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Interfaces;

internal interface IWashingMachineReadOnlyRepository
{
    Task<WashingMachine?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<WashingMachine?> GetByLineCodeAsync(string lineCode, CancellationToken cancellationToken);
    Task<IReadOnlyList<WashingMachine>> GetWashingMachinesByCodesAsync(string[] codes, CancellationToken cancellationToken);
}