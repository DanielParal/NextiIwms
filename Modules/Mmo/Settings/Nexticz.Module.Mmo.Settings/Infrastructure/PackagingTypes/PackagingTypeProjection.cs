
using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.PackagingTypes;

public class PackagingTypeProjection : SingleStreamProjection<PackagingType, Guid>
{
    public void Apply(IEvent<PackagingTypeCreatedEvent> @event, PackagingType depositor)
    {
        depositor.Apply(@event.Data);
    }
    
    public void Apply(IEvent<PackagingTypeNameUpdatedEvent> @event, PackagingType depositor)
    {
        depositor.Apply(@event.Data);
    }
    
    public PackagingType? Apply(IEvent<PackagingTypeDeletedEvent> @event, PackagingType depositor)
    {
        return null;
    }
    
}