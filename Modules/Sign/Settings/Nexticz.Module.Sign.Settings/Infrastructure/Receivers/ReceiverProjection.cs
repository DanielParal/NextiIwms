using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Receivers;

public class ReceiverProjection : SingleStreamProjection<Receiver, Guid>
{
    public ReceiverProjection()
    {
        DeleteEvent<ReceiverDeletedEvent>();
    }
    
    public void Apply(IEvent<ReceiverCreatedEvent> @event, Receiver partner)
    {
        partner.Apply(@event.Data);
    }
    
    public void Apply(IEvent<ReceiverUpdatedEvent> @event, Receiver partner)
    {
        partner.Apply(@event.Data);
    }
}