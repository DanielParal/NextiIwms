using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Depositors;

public class DepositorProjection : SingleStreamProjection<Depositor, Guid>
{
    public DepositorProjection()
    {
        DeleteEvent<DepositorDeletedEvent>();
    }
    
    public void Apply(IEvent<DepositorCreatedEvent> @event, Depositor depositor)
    {
        depositor.Apply(@event.Data);
    }
    
    public void Apply(IEvent<DepositorUpdatedEvent> @event, Depositor depositor)
    {
        depositor.Apply(@event.Data);
    }
}