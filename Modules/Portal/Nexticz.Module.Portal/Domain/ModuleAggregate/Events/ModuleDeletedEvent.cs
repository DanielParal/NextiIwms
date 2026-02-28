using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Portal.Domain.ModuleAggregate.Events;

public record ModuleDeletedEvent(
    Guid Id, DateTimeOffset DeletedAt) : IMartenEvent;