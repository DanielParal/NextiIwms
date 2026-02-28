using ErrorOr;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.DetachSisterBatch;

internal record DetachSisterBatchCommand(Guid BatchId, string LineQueueCode) : IPlanningCommand<ErrorOr<Success>>;