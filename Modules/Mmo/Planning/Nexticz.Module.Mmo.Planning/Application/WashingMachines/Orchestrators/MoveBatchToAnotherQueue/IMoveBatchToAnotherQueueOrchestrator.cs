using ErrorOr;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Orchestrators.MoveBatchToAnotherQueue;

internal interface IMoveBatchToAnotherQueueOrchestrator
{
    Task<ErrorOr<Success>> OrchestrateSingleBatchAsync(Batch batch, string upperCurrentLineQueueCode, 
        string newUpperLineQueueCode, CancellationToken cancellationToken);
    
    Task<ErrorOr<Success>> OrchestrateSisterBatchesAsync(Batch batch, Batch sisterBatch, 
        string upperCurrentLineQueueCode, string newUpperLineQueueCode, CancellationToken cancellationToken);
}