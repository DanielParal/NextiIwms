using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Portal.Domain.ModuleAggregate.Events;

public record ModuleCreatedEvent(
    Guid Id, string Name, string Icon, string BaseUrl, bool IsActive, int SortOrder, DateTimeOffset CreatedAt) : IMartenEvent;