using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Portal.Domain.ModuleAggregate.Events;

namespace Nexticz.Module.Portal.Infrastructure.Modules;

public class ModuleProjection : SingleStreamProjection<Domain.ModuleAggregate.Module, Guid>
{
    public ModuleProjection()
    {
        DeleteEvent<ModuleDeletedEvent>();
    }
    
    public void Apply(IEvent<ModuleCreatedEvent> @event, Domain.ModuleAggregate.Module module)
    {
        module.Apply(@event.Data);
    }
    
    public void Apply(IEvent<ModuleUpdatedEvent> @event, Domain.ModuleAggregate.Module module)
    {
        module.Apply(@event.Data);
    }
    
    public void Apply(IEvent<ModuleOrderChangedEvent> @event, Domain.ModuleAggregate.Module module)
    {
        module.Apply(@event.Data);
    }
}