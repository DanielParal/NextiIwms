using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.WashingMachines;

public class WashingMachineProjection : SingleStreamProjection<WashingMachine, Guid>
{
    public void Apply(IEvent<WashingMachineCreatedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
    
    public void Apply(IEvent<WashingMachineUpdatedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
}