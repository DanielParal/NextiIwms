using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

public record SisterBatchInQueueMovedEvent(
    string WashingMachineCode,
    Guid BatchId,
    string LineQueueCode,
    int NewIndex,
    Guid SisterBatchId,
    string SisterLineQueueCode,
    int SisterNewIndex) : IMartenEvent;