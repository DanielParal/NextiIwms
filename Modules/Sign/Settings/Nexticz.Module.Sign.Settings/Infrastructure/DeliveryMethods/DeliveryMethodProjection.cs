using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Infrastructure.DeliveryMethods;

public class DeliveryMethodProjection : SingleStreamProjection<DeliveryMethod, Guid>
{
    public DeliveryMethodProjection()
    {
        DeleteEvent<DeliveryMethodDeletedEvent>();
    }
    
    public void Apply(IEvent<DeliveryMethodCreatedEvent> @event, DeliveryMethod deliveryMethod)
    {
        deliveryMethod.Apply(@event.Data);
    }
    
    public void Apply(IEvent<DeliveryMethodUpdatedEvent> @event, DeliveryMethod deliveryMethod)
    {
        deliveryMethod.Apply(@event.Data);
    }
}