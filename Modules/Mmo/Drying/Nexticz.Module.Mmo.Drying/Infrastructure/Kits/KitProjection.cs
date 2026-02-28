using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Drying.Domain.KitAggregate;
using Nexticz.Module.Mmo.Drying.Domain.KitAggregate.Events;

namespace Nexticz.Module.Mmo.Drying.Infrastructure.Kits;

public class KitProjection : SingleStreamProjection<Kit, Guid>
{
    public void Apply(IEvent<KitCreatedEvent> @event, Kit kit)
    {
        kit.Apply(@event.Data);
    }
    
    public void Apply(IEvent<KitToDryingSectionTransferredEvent> @event, Kit kit)
    {
        kit.Apply(@event.Data);
    }
    
    public Kit? Apply(IEvent<KitFinishedEvent> @event, Kit kit)
    {
        return null;
    }
}