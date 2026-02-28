using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Portal.Domain.ModuleAggregate.Events;

public record ModuleOrderChangedEvent(Guid Id, int NewSortOrder, DateTimeOffset ChangedAt) : IMartenEvent;