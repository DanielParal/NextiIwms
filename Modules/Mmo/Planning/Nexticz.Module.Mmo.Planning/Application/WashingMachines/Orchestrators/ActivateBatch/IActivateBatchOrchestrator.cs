using ErrorOr;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Orchestrators.ActivateBatch;

internal interface IActivateBatchOrchestrator
{
    Task<ErrorOr<ActivateBatchResponse>> OrchestrateAsync(
        Batch batch, string washingMachineCode, string upperLineQueueCode, 
        Batch? currentBatchInWashing, string currentBatchInWashingLineCode,
        Batch? currentBatchInWashingInOtherLine, string? currentBatchInWashingInOtherLineLineCode,
        CancellationToken cancellationToken);
}