using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate.Events;

namespace Nexticz.Module.Mmo.Reporting.Infrastructure.WashingMachineSpeeds;

public class WashingMachineSpeedProjection : SingleStreamProjection<WashingMachineSpeed, Guid>
{
    public void Apply(IEvent<WashingMachineSpeedStartedEvent> @event, WashingMachineSpeed speed)
    {
        speed.Apply(@event.Data);
    }
    
    public void Apply(IEvent<WashingMachineSpeedEndedEvent> @event, WashingMachineSpeed speed)
    {
        speed.Apply(@event.Data);
    }
}