using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;

public record BatchFinishedEvent(Guid BatchId, DateTimeOffset DateFinished) : IMartenEvent;