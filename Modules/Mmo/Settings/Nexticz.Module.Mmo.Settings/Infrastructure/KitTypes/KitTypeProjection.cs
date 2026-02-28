using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.KitTypes;

public class KitTypeProjection : SingleStreamProjection<KitType, Guid>
{
    public void Apply(IEvent<KitTypeCreatedEvent> @event, KitType kitType)
    {
        kitType.Apply(@event.Data);
    }
    
    public void Apply(IEvent<KitTypeNameUpdatedEvent> @event, KitType kitType)
    {
        kitType.Apply(@event.Data);
    }
    
    public KitType? Apply(IEvent<KitTypeDeletedEvent> @event, KitType kitType)
    {
        return null;
    }

}