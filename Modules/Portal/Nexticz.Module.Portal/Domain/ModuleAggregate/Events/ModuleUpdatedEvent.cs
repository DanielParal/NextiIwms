using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Portal.Domain.ModuleAggregate.Events;

public record ModuleUpdatedEvent(
    Guid Id, string Name, string Icon, string BaseUrl, bool IsActive, DateTimeOffset UpdatedAt) : IMartenEvent;