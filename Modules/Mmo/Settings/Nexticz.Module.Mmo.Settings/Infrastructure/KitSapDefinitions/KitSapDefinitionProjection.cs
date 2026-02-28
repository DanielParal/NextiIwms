using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.KitSapDefinitions;

public class KitSapDefinitionProjection : SingleStreamProjection<KitSapDefinition, Guid>
{
    public void Apply(IEvent<KitSapDefinitionCreatedEvent> @event, KitSapDefinition kitSapDefinition)
    {
        kitSapDefinition.Apply(@event.Data);
    }
    
    public void Apply(IEvent<KitSapDefinitionNameUpdatedEvent> @event, KitSapDefinition kitSapDefinition)
    {
        kitSapDefinition.Apply(@event.Data);
    }
    
    public KitSapDefinition? Apply(IEvent<KitSapDefinitionDeletedEvent> @event, KitSapDefinition kitSapDefinition)
    {
        return null;
    }

}