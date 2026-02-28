using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

public record BatchInQueueMovedEvent(
    Guid BatchId,
    string WashingMachineCode,
    string LineQueueCode,
    int NewIndex) : IMartenEvent;