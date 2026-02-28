using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.Settings.Domain.LocationAggregate;
using Nexticz.Module.Sign.Settings.Domain.LocationAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Locations;

public class LocationProjection : SingleStreamProjection<Location, Guid>
{
    public LocationProjection()
    {
        DeleteEvent<LocationDeletedEvent>();
    }
    
    public void Apply(IEvent<LocationCreatedEvent> @event, Location location)
    {
        location.Apply(@event.Data);
    }
    
    public void Apply(IEvent<LocationUpdatedEvent> @event, Location location)
    {
        location.Apply(@event.Data);
    }
}