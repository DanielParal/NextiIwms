using ErrorOr;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Orchestrators.SplitBatch;

internal interface ISplitBatchOrchestrator
{
    Task<ErrorOr<Success>> OrchestrateSingleBatchAsync(Batch batch, string upperLineQueueCode, 
        int countToChange, int kitsCountToCreate, CancellationToken cancellationToken);
    
    Task<ErrorOr<Success>> OrchestrateSisterBatchesAsync(Batch batch, Batch sisterBatch, 
        string upperLineQueueCode, int countToChange, int kitsCountToCreate, CancellationToken cancellationToken);
}