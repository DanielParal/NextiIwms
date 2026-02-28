using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate.Events;

namespace Nexticz.Module.Cuzk.Infrastructure.AddressLocations;

public class AddressLocationProjection : SingleStreamProjection<AddressLocation, Guid>
{
    public AddressLocationProjection()
    {
        DeleteEvent<AddressLocationDeletedEvent>();
    }
    
    public void Apply(IEvent<AddressLocationCreatedEvent> @event, AddressLocation addressLocation)
    {
        addressLocation.Apply(@event.Data);
    }
    
    public void Apply(IEvent<AddressLocationUpdatedEvent> @event, AddressLocation addressLocation)
    {
        addressLocation.Apply(@event.Data);
    }
}