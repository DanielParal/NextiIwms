using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Infrastructure.DepositorGroups;

public class DepositorGroupProjection : SingleStreamProjection<DepositorGroup, Guid>
{
    public DepositorGroupProjection()
    {
        DeleteEvent<DepositorGroupDeletedEvent>();
    }
    
    public void Apply(IEvent<DepositorGroupCreatedEvent> @event, DepositorGroup depositorGroup)
    {
        depositorGroup.Apply(@event.Data);
    }
    
    public void Apply(IEvent<DepositorGroupUpdatedEvent> @event, DepositorGroup depositorGroup)
    {
        depositorGroup.Apply(@event.Data);
    }
}