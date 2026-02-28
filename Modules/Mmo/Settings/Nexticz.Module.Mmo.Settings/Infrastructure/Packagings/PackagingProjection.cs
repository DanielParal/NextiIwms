using JasperFx.Events;
using Marten.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate.Events;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Packagings;

public class PackagingProjection : SingleStreamProjection<Packaging, Guid>
{
    public void Apply(IEvent<PackagingCreatedEvent> @event, Packaging packaging)
    {
        packaging.Apply(@event.Data);
    }
    
    public void Apply(IEvent<PackagingUpdatedEvent> @event, Packaging packaging)
    {
        packaging.Apply(@event.Data);
    }
    
    public void Apply(IEvent<PackagingWashingMachineSpeedCreatedEvent> @event, Packaging packaging)
    {
        packaging.Apply(@event.Data);
    }

    
    public void Apply(IEvent<PackagingWashingMachineSpeedDeletedEvent> @event, Packaging packaging)
    {
        packaging.Apply(@event.Data);
    }
    
    public Packaging? Apply(IEvent<PackagingDeletedEvent> @event, Packaging packaging)
    {
        return null;
    }
}