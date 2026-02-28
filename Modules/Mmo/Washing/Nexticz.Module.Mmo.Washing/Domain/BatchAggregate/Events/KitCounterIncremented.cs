using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;

public record KitCounterIncremented(
    Guid KitId,
    Guid BatchId,
    Guid? SisterKitId,
    Guid? SisterBatchId,
    int CompletedKitsCount) : IMartenEvent;