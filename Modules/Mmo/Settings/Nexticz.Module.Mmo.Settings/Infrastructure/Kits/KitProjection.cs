using JasperFx.Events;
using Marten.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate.Events;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Kits;

public class KitProjection : SingleStreamProjection<Kit, Guid>
{
    public void Apply(IEvent<KitCreatedEvent> @event, Kit kit)
    {
        kit.Apply(@event.Data);
    }
    
    public void Apply(IEvent<KitUpdatedEvent> @event, Kit kit)
    {
        kit.Apply(@event.Data);
    }
    
    public void Apply(IEvent<KitInstructionUploadedEvent> @event, Kit kit)
    {
        kit.Apply(@event.Data);
    }
    
    public void Apply(IEvent<KitInstructionDeletedEvent> @event, Kit kit)
    {
        kit.Apply(@event.Data);
    }
    
    public Kit? Apply(IEvent<KitDeletedEvent> @event, Kit kit)
    {
        return null;
    }
}