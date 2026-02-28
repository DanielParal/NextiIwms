using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.SpecialInformations;

public class SpecialInformationProjection : SingleStreamProjection<SpecialInformation, Guid>
{
    public void Apply(IEvent<SpecialInformationCreatedEvent> @event, SpecialInformation specialInformation)
    {
        specialInformation.Apply(@event.Data);
    }
    
    public void Apply(IEvent<SpecialInformationUpdatedEvent> @event, SpecialInformation specialInformation)
    {
        specialInformation.Apply(@event.Data);
    }
    
    public void Apply(IEvent<SpecialInformationImageUploadedEvent> @event, SpecialInformation specialInformation)
    {
        specialInformation.Apply(@event.Data);
    }
    
    public void Apply(IEvent<SpecialInformationImageDeletedEvent> @event, SpecialInformation specialInformation)
    {
        specialInformation.Apply(@event.Data);
    }
    
    public SpecialInformation? Apply(IEvent<SpecialInformationDeletedEvent> @event, SpecialInformation specialInformation)
    {
        return null;
    }
}