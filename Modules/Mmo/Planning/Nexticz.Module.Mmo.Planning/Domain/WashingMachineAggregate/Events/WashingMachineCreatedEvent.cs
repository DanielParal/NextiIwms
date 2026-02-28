using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

public class WashingMachineCreatedEvent(
    Guid id,
    string code,
    WashingMachineStatus status,
    bool isOneLineMachine,
    LineQueue[] lineQueues) : EventWithCode(code), IMartenEvent
{
    public Guid Id { get; } = id;
    public WashingMachineStatus Status { get; } = status;
    public bool IsOneLineMachine { get; } = isOneLineMachine;
    public LineQueue[] LineQueues { get; } = lineQueues;
}