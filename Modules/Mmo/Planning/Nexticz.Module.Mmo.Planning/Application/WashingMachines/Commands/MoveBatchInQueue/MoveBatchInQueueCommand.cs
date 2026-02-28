using ErrorOr;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.MoveBatchInQueue;

internal record MoveBatchInQueueCommand(string LineQueueCode, Guid BatchId, InQueueMovementContract Movement) : IPlanningCommand<ErrorOr<Success>>;