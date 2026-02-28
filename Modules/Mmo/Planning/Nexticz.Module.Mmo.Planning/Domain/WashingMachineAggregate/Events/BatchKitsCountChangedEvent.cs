using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

public record BatchKitsCountChangedEvent(
    Guid BatchId,
    string WashingMachineCode,
    string LineQueueCode,
    int CountToChange) : IMartenEvent;