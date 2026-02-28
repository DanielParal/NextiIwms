using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate.Events;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.WashingMachineSpeeds;

public class WashingMachineSpeedProjection : SingleStreamProjection<WashingMachineSpeed, Guid>
{
    public void Apply(IEvent<WashingMachineSpeedCreatedEvent> @event, WashingMachineSpeed washingMachineSpeed)
    {
        washingMachineSpeed.Apply(@event.Data);
    }
    
    public void Apply(IEvent<WashingMachineSpeedUpdatedEvent> @event, WashingMachineSpeed washingMachineSpeed)
    {
        washingMachineSpeed.Apply(@event.Data);
    }
}