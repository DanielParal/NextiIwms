using ErrorOr;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.FinishBatch;

internal record FinishBatchCommand(Guid BatchId, string LineQueueCode, DateTimeOffset? DateActivatedNextBatch = null) 
    : IPlanningCommand<ErrorOr<(Guid FinishedBatchId, Guid? FinishedSisterBatchId)>>;