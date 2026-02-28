using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Reporting.Domain.WorkerLinePresenceAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.WorkerLinePresenceAggregate.Events;

namespace Nexticz.Module.Mmo.Reporting.Infrastructure.WorkerLinePresences;

public class WorkerLinePresenceProjection : SingleStreamProjection<WorkerLinePresence, Guid>
{
    public void Apply(IEvent<WorkerEnteredLineEvent> @event, WorkerLinePresence workerLinePresence)
    {
        workerLinePresence.Apply(@event.Data);
    }
    
    public void Apply(IEvent<WorkerLeftLineEvent> @event, WorkerLinePresence workerLinePresence)
    {
        workerLinePresence.Apply(@event.Data);
    }
}