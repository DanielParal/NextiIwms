using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

public record BatchWashingStartedEvent(
    Guid BatchId,
    string WashingMachineCode,
    string LineQueueCode,
    DateTimeOffset WashingStartedAt) : IMartenEvent;