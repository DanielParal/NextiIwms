using ErrorOr;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.MoveBatchToAnotherQueue;

internal record MoveBatchToAnotherQueueCommand(Guid BatchId, string CurrentLineQueueCode, string NewLineQueueCode) : IPlanningCommand<ErrorOr<Success>>;