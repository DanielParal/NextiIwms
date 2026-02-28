using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate.Events;

namespace Nexticz.Module.EmailSender.Infrastructure.EmailMessages;

public class EmailMessageProjection : SingleStreamProjection<EmailMessage, Guid>
{
    public void Apply(IEvent<EmailMessageCreatedEvent> @event, EmailMessage emailMessage)
    {
        emailMessage.Apply(@event.Data);
    }
    
    public void Apply(IEvent<EmailMessageScheduledEvent> @event, EmailMessage emailMessage)
    {
        emailMessage.Apply(@event.Data);
    }
    
    public void Apply(IEvent<EmailMessageSentEvent> @event, EmailMessage emailMessage)
    {
        emailMessage.Apply(@event.Data);
    }
    
    public void Apply(IEvent<EmailMessageFailedEvent> @event, EmailMessage emailMessage)
    {
        emailMessage.Apply(@event.Data);
    }
}