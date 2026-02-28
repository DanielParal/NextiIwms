using Nexticz.Module.Mmo.Reporting.Domain.WorkerEntity;
using Nexticz.Module.Mmo.Reporting.Domain.WorkerLinePresenceAggregate.Events;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Reporting.Domain.WorkerLinePresenceAggregate;

public class WorkerLinePresence : AggregateRoot
{
    public string LineCode { get; private set; }
    public Worker? Worker { get; private set; }
    public DateTimeOffset? LastEnteredDate { get; private set; }
    
    private WorkerLinePresence() {}
    
    public WorkerLinePresence(
        string lineCode,
        Worker? worker,
        DateTimeOffset? lastEnteredDate,
        Guid? id = null) : base(id ?? GenerateIdFromString($"WorkerLinePresence_{lineCode}"))
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