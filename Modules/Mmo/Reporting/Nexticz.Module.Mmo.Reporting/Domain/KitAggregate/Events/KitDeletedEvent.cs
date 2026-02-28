using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Reporting.Domain.KitAggregate.Events;

public record KitDeletedEvent(Guid Id, DateTimeOffset DeletedAt, string DeletedBy) : IMartenEvent;