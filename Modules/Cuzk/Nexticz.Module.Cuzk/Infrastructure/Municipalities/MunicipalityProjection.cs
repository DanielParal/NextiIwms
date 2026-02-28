using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate.Events;

namespace Nexticz.Module.Cuzk.Infrastructure.Municipalities;

public class MunicipalityProjection : SingleStreamProjection<Municipality, Guid>
{
    public MunicipalityProjection()
    {
        DeleteEvent<MunicipalityDeletedEvent>();
    }
    
    public void Apply(IEvent<MunicipalityCreatedEvent> @event, Municipality municipality)
    {
        municipality.Apply(@event.Data);
    }
    
    public void Apply(IEvent<MunicipalityUpdatedEvent> @event, Municipality municipality)
    {
        municipality.Apply(@event.Data);
    }
}