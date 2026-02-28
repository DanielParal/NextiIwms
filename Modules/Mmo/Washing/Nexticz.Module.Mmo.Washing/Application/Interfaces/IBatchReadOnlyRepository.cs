using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Interfaces;

internal interface IBatchReadOnlyRepository
{
    Task<Batch?> GetBatchByLineCodeAsync(string lineCode, CancellationToken cancellationToken);
    Task<Batch?> GetBatchByKitIdAsync(Guid kitId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Batch>> GetBatchesByWashingMachineCodeAsync(string washingMachineCode, CancellationToken cancellationToken);
}