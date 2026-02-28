using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate.Events;

public record DocumentTemplateDeletedEvent(Guid Id, DateTimeOffset DeletedAt) : IMartenEvent;