using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;
using Nexticz.Module.Mmo.Washing.Domain.KitWashCycleEntity;

namespace Nexticz.Module.Mmo.Washing.Application.Batches;

internal static class BatchContractFactory
{
    public static BatchContract Create(Batch batch, KitWashCycle kitWashCycle)
    {
        var kitWashCycleContract = 
            KitWashCycleContractFactory.Create(kitWashCycle);

        var printings = batch.Printings
            .Select(x =>
                new PrintingContract(x.Id, x.BatchId, x.KitId, x.DatePrinted, (PrintingStatusContract)x.Status,
                    (PrintingTypeContract)x.Type, x.FailureReason))
            .ToArray();
        
        var batchContract = new BatchContract(
            batch.Id,
            batch.SisterBatchId,
            batch.WashingMachineCode,
            batch.LineCode,
            batch.KitCode,
            batch.KitNumber,
            batch.PackagingCode,
            batch.DefiningPackagingCode,
            batch.OptimalKitDuration,
            [kitWashCycleContract],
            printings);
        
        return batchContract;
    }
}