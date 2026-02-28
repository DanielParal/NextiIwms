using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

public class WashingMachineStatusUpdatedEvent(
    string code,
    WashingMachineStatus status,
    LineQueue[] lineQueues) : EventWithCode(code), IMartenEvent
{
    public string Code { get; } = code;
    public WashingMachineStatus Status { get; } = status;
    public LineQueue[] LineQueues { get; } = lineQueues;
}