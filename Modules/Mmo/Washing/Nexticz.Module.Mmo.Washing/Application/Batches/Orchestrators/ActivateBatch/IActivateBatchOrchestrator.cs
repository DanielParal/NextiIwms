using ErrorOr;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Orchestrators.ActivateBatch;

internal interface IActivateBatchOrchestrator
{
    Task<ErrorOr<Success>> OrchestrateAsync(
        Guid? finishedBatchId, Guid? finishedSisterBatchId,
        int requestedWashingMachineSpeed, SpeedLevel requestedWashingMachineSpeedLevel,
        BatchActivatedResponse activatedBatch, BatchActivatedResponse? activatedSisterBatch,
        DateTimeOffset dateActivated, CancellationToken cancellationToken);
}