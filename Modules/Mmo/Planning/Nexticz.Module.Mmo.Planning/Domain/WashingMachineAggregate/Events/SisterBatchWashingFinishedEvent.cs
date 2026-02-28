using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

public record SisterBatchWashingFinishedEvent(
    Guid BatchId,
    Guid SisterBatchId,
    string WashingMachineCode,
    string LineQueueCode,
    string SisterLineQueueCode,
    DateTimeOffset WashingFinishedAt) : IMartenEvent;