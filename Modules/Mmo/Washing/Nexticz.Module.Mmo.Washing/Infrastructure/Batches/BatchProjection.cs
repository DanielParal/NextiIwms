using JasperFx.Events;
using Marten.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.Batches;

public class BatchProjection : SingleStreamProjection<Batch, Guid>
{
    public BatchProjection()
    {
        DeleteEvent<BatchFinishedEvent>();
    }
    
    public void Apply(IEvent<BatchCreatedEvent> @event, Batch batch)
    {
        batch.Apply(@event.Data);
    }
    
    public void Apply(IEvent<KitWashCycleFinishedEvent> @event, Batch batch)
    {
        batch.Apply(@event.Data);
    }
    
    public void Apply(IEvent<SisterBatchDetachedEvent> @event, Batch batch)
    {
        batch.Apply(@event.Data);
    }
    
    public void Apply(IEvent<PrintingCreatedEvent> @event, Batch batch)
    {
        batch.Apply(@event.Data);
    }
    
    public void Apply(IEvent<SpecialInformationConfirmedEvent> @event, Batch batch)
    {
        batch.Apply(@event.Data);
    }
    
    public void Apply(IEvent<BatchPlannedKitsCountChanged> @event, Batch batch)
    {
        batch.Apply(@event.Data);
    }
}