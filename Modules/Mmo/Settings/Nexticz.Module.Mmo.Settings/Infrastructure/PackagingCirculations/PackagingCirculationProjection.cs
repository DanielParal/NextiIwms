using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.PackagingCirculations;

public class PackagingCirculationProjection : SingleStreamProjection<PackagingCirculation, Guid>
{
    public void Apply(IEvent<PackagingCirculationCreatedEvent> @event, PackagingCirculation packagingCirculation)
    {
        packagingCirculation.Apply(@event.Data);
    }
    
    public void Apply(IEvent<PackagingCirculationNameUpdatedEvent> @event, PackagingCirculation packagingCirculation)
    {
        packagingCirculation.Apply(@event.Data);
    }
    
    public PackagingCirculation? Apply(IEvent<PackagingCirculationDeletedEvent> @event, PackagingCirculation packagingCirculation)
    {
        return null;
    }
}