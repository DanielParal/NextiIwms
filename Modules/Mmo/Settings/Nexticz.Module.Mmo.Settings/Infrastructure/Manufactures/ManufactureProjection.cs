using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Manufactures;

public class ManufactureProjection : SingleStreamProjection<Manufacture, Guid>
{
    public void Apply(IEvent<ManufactureCreatedEvent> @event, Manufacture manufacture)
    {
        manufacture.Apply(@event.Data);
    }
    
    public void Apply(IEvent<ManufactureNameUpdatedEvent> @event, Manufacture manufacture)
    {
        manufacture.Apply(@event.Data);
    }
    
    public Manufacture? Apply(IEvent<ManufactureDeletedEvent> @event, Manufacture manufacture)
    {
        return null;
    }
}