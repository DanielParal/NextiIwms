using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateWashingMachine;

internal record CreateWashingMachineCommand(
    string WashingMachineCode, WashingMachineStatus Status, LineQueue[] LineQueues) : IPlanningCommand<ErrorOr<Created>>;