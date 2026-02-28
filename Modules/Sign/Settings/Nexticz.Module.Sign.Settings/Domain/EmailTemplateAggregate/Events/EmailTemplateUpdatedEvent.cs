using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate.Events;

public record EmailTemplateUpdatedEvent(Guid Id, string Code, string Name, string Subject, string HtmlBody, string TextBody) : IMartenEvent;