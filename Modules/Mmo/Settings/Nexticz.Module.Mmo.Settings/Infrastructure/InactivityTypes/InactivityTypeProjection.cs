using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.InactivityTypes;

public class InactivityTypeProjection : SingleStreamProjection<InactivityType, Guid>
{
    public InactivityTypeProjection()
    {
        DeleteEvent<InactivityTypeDeletedEvent>();
    }
    
    public void Apply(IEvent<InactivityTypeCreatedEvent> @event, InactivityType inactivityType)
    {
        inactivityType.Apply(@event.Data);
    }
    
    public void Apply(IEvent<InactivityTypeUpdatedEvent> @event, InactivityType inactivityType)
    {
        inactivityType.Apply(@event.Data);
    }
}