using ErrorOr;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.ActivateBatch;

internal record ActivateBatchCommand(Guid BatchId, string LineQueueCode) : IPlanningCommand<ErrorOr<ActivateBatchResponse>>;