using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Depositors;

public class DepositorProjection : SingleStreamProjection<Depositor, Guid>
{
    public void Apply(IEvent<DepositorCreatedEvent> @event, Depositor depositor)
    {
        depositor.Apply(@event.Data);
    }
    
    public void Apply(IEvent<DepositorUpdatedEvent> @event, Depositor depositor)
    {
        depositor.Apply(@event.Data);
    }
    
    public Depositor? Apply(IEvent<DepositorDeletedEvent> @event, Depositor depositor)
    {
        return null;
    }
}