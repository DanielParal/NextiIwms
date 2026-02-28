using ErrorOr;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.StartBatchWashing;

internal record StartBatchWashingCommand(Guid BatchId, string LineQueueCode, DateTimeOffset DateActivatedBatch) 
    : IPlanningCommand<ErrorOr<(BatchActivatedResponse BatchStarted, BatchActivatedResponse? SisterBatchStarted)>>;