using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Workers;

public class WorkerProjection : SingleStreamProjection<Worker, Guid>
{
    public void Apply(IEvent<WorkerCreatedEvent> @event, Worker worker)
    {
        worker.Apply(@event.Data);
    }
    
    public void Apply(IEvent<WorkerUpdatedEvent> @event, Worker worker)
    {
        worker.Apply(@event.Data);
    }
    
    public Worker? Apply(IEvent<WorkerDeletedEvent> @event, Worker worker)
    {
        return null;
    }
}