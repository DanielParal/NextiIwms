using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;
using Nexticz.Module.Mmo.Washing.Domain.LastEnteredWorkerOnLineAggregate.Events;
using Nexticz.Module.Mmo.Washing.Domain.WorkerEntity;

namespace Nexticz.Module.Mmo.Washing.Domain.LastEnteredWorkerOnLineAggregate;

public class LastEnteredWorkerOnLine : AggregateRoot
{
    public string LineCode { get; private set; }
    public Worker? Worker { get; private set; }
    public DateTimeOffset? LastEnteredDate { get; private set; }
    
    private LastEnteredWorkerOnLine() {}
    
    public LastEnteredWorkerOnLine(
        string lineCode,
        Worker? worker,
        DateTimeOffset? lastEnteredDate,
        Guid? id = null) : base(id ?? GenerateIdFromString(lineCode))
    {
        LineCode = lineCode;
        Worker = worker;
        LastEnteredDate = lastEnteredDate;
    }
    
    public void Apply(WorkerEnteredLineEvent @event)
    {
        Id = @event.Id;
        LineCode = @event.LineCode;
        Worker = new Worker(@event.WorkerName, @event.WorkerId);
        LastEnteredDate = @event.EnteredAt;
    }
    
    public void Apply(WorkerLeftLineEvent @event)
    {
        LineCode = @event.LineCode;
        Worker = null;
        LastEnteredDate = null;
    }
}