using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Infrastructure.EmailTemplates;

public class EmailTemplateProjection : SingleStreamProjection<EmailTemplate, Guid>
{
    public EmailTemplateProjection()
    {
        DeleteEvent<EmailTemplateDeletedEvent>();
    }
    
    public void Apply(IEvent<EmailTemplateCreatedEvent> @event, EmailTemplate emailTemplate)
    {
        emailTemplate.Apply(@event.Data);
    }
    
    public void Apply(IEvent<EmailTemplateUpdatedEvent> @event, EmailTemplate emailTemplate)
    {
        emailTemplate.Apply(@event.Data);
    }
}