using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.Interfaces;

internal interface IWashingMachineReadOnlyRepository
{
    Task<WashingMachine?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<WashingMachine?> GetByLineQueueCodeAsync(string lineQueueCode, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<WashingMachine>> GetAllByCodesAsync(string[] codes, CancellationToken cancellationToken);
}