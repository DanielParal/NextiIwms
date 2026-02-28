using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Reporting.Domain.KitAggregate.Events;

public record KitCommentChangedEvent(Guid Id, string Comment, DateTimeOffset UpdatedAt, string UpdatedBy) : IMartenEvent;