using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines;

internal class BatchContractFactory
{
    public static BatchContract Create(Batch batch)
    {
        return new BatchContract(
            batch.Id,
            batch.SisterBatchId,
            batch.HasSisterBatch,
            batch.DepositorCode,
            batch.KitCode,
            batch.KitNumber,
            batch.KitsCount,
            batch.KitsFinished,
            batch.KitsLeft,
            batch.PackagingCode,
            batch.PackagingHeight,
            batch.DefiningPackagingCode,
            batch.KitSapDefinitionCode,
            batch.OptimalKitDuration,
            batch.OptimalBatchDuration,
            (BatchStatusContract)batch.Status
        );
    }
}