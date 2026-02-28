using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Infrastructure.EmailConfigurations;

public class EmailConfigurationProjection : SingleStreamProjection<EmailConfiguration, Guid>
{
    public EmailConfigurationProjection()
    {
        DeleteEvent<EmailConfigurationDeletedEvent>();
        DeleteEvent<EmailConfigurationDeletedV2Event>();
    }
    
    public void Apply(IEvent<EmailConfigurationCreatedEvent> @event, EmailConfiguration emailConfiguration)
    {
        emailConfiguration.Apply(@event.Data);
    }
    
    public void Apply(IEvent<EmailConfigurationUpdatedEvent> @event, EmailConfiguration emailConfiguration)
    {
        emailConfiguration.Apply(@event.Data);
    }
    
    public void Apply(IEvent<EmailConfigurationCreatedV2Event> @event, EmailConfiguration emailConfiguration)
    {
        emailConfiguration.Apply(@event.Data);
    }
    
    public void Apply(IEvent<EmailConfigurationUpdatedV2Event> @event, EmailConfiguration emailConfiguration)
    {
        emailConfiguration.Apply(@event.Data);
    }
}