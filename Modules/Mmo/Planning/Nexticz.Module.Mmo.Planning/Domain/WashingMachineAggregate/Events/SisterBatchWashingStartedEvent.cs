using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

public record SisterBatchWashingStartedEvent(
    Guid BatchId,
    Guid SisterBatchId,
    string WashingMachineCode,
    string LineQueueCode,
    string SisterLineQueueCode,
    DateTimeOffset WashingStartedAt) : IMartenEvent;