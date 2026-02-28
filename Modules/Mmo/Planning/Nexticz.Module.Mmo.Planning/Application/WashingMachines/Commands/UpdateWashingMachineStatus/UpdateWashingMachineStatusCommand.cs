using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.UpdateWashingMachineStatus;

internal record UpdateWashingMachineStatusCommand(
    string WashingMachineCode, WashingMachineStatus Status, LineQueue[] LineQueues) : IPlanningCommand<ErrorOr<Updated>>;