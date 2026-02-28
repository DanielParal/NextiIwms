using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;

public record BatchPlannedKitsCountChanged(
    Guid BatchId,
    int PlannedKitsCount) : IMartenEvent;