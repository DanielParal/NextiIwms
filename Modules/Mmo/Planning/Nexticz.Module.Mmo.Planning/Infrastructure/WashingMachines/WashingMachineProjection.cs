using JasperFx.Events;
using Marten.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

namespace Nexticz.Module.Mmo.Planning.Infrastructure.WashingMachines;

public class WashingMachineProjection : SingleStreamProjection<WashingMachine, Guid>
{
    public void Apply(IEvent<WashingMachineCreatedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
    
    public void Apply(IEvent<WashingMachineStatusUpdatedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
    
    public void Apply(IEvent<BatchCreatedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
    
    public void Apply(IEvent<SisterBatchCreatedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
    
    public void Apply(IEvent<BatchRemovedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
    
    public void Apply(IEvent<SisterBatchRemovedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
    
    public void Apply(IEvent<BatchKitsCountChangedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
    
    public void Apply(IEvent<SisterBatchKitsCountChangedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
    
    public void Apply(IEvent<BatchInQueueMovedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
    
    public void Apply(IEvent<SisterBatchInQueueMovedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
    
    public void Apply(IEvent<SisterBatchDetachedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
    
    public void Apply(IEvent<BatchWashingStartedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
    
    public void Apply(IEvent<SisterBatchWashingStartedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
    
    public void Apply(IEvent<BatchWashingFinishedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
    
    public void Apply(IEvent<SisterBatchWashingFinishedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
    
    public void Apply(IEvent<BatchKitFinishedEvent> @event, WashingMachine washingMachine)
    {
        washingMachine.Apply(@event.Data);
    }
}