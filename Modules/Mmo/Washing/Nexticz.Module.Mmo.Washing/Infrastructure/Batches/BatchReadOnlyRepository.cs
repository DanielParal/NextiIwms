using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.Batches;

internal class BatchReadOnlyRepository(
    IWashingReadOnlyEventStoreRepository readOnlyRepository
    )
    : IBatchReadOnlyRepository
{
    public async Task<Batch?> GetBatchByLineCodeAsync(string lineCode, CancellationToken cancellationToken)
    {
        return await readOnlyRepository.GetFirstByConditionAsync<Batch>(
            x => 
                x.LineCode.Equals(lineCode, StringComparison.InvariantCultureIgnoreCase),
            cancellationToken);
    }
    
    public async Task<Batch?> GetBatchByKitIdAsync(Guid kitId, CancellationToken cancellationToken)
    {
        return await readOnlyRepository.GetFirstByConditionAsync<Batch>(
            x => 
                x.KitWashCycles.Any(kwc => kwc.Id == kitId),
            cancellationToken);
    }

    public async Task<IReadOnlyList<Batch>> GetBatchesByWashingMachineCodeAsync(string washingMachineCode, CancellationToken cancellationToken)
    {
        return await readOnlyRepository.GetAllByConditionAsync<Batch>(
            x => 
                x.WashingMachineCode.Equals(washingMachineCode, StringComparison.InvariantCultureIgnoreCase),
            cancellationToken);
    }
}