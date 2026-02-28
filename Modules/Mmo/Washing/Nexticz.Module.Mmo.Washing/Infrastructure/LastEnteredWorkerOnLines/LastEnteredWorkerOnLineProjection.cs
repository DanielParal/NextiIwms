using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Washing.Domain.LastEnteredWorkerOnLineAggregate;
using Nexticz.Module.Mmo.Washing.Domain.LastEnteredWorkerOnLineAggregate.Events;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.LastEnteredWorkerOnLines;

public class LastEnteredWorkerOnLineProjection : SingleStreamProjection<LastEnteredWorkerOnLine, Guid>
{
    public void Apply(IEvent<WorkerEnteredLineEvent> @event, LastEnteredWorkerOnLine lastEnteredWorkerOnLine)
    {
        lastEnteredWorkerOnLine.Apply(@event.Data);
    }
    
    public void Apply(IEvent<WorkerLeftLineEvent> @event, LastEnteredWorkerOnLine lastEnteredWorkerOnLine)
    {
        lastEnteredWorkerOnLine.Apply(@event.Data);
    }
}