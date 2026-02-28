using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate.Events;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.WashingMachineSoses;

public class WashingMachineSosProjection : SingleStreamProjection<WashingMachineSos, Guid>
{
    public void Apply(IEvent<WashingMachineSosCalledEvent> @event, WashingMachineSos washingMachineSos)
    {
        washingMachineSos.Apply(@event.Data);
    }
    
    public void Apply(IEvent<WashingMachineSosResolvedEvent> @event, WashingMachineSos washingMachineSos)
    {
        washingMachineSos.Apply(@event.Data);
    }
}