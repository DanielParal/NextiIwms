using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate.Events;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

public class Shift : AggregateRoot
{
    public string Name { get; private set; }
    public bool IsLast { get; private set; }
    public bool IsNextToLast { get; private set; }
    public ShiftSchedule Schedule { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public string? ApprovedBy { get; private set; }
    public bool IsApproved => ApprovedAt.HasValue;
    public WashingMachine[] WashingMachines { get; private set; }

    // We need private constructor due to Marten deserialization
    private Shift() {}

    public Shift(
        string name,
        bool isLast,
        bool isNextToLast,
        ShiftSchedule schedule,
        DateTimeOffset createdAt,
        WashingMachine[] washingMachines,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        Name = name;
        IsLast = isLast;
        IsNextToLast = isNextToLast;
        Schedule = schedule;
        CreatedAt = createdAt;
        WashingMachines = washingMachines;
        ApprovedAt = null;
        ApprovedBy = null;
    }
    
    public void Approve(string approvedBy, DateTimeOffset approvedAt)
    {
        ApprovedAt = approvedAt;
        ApprovedBy = approvedBy;
    }
    
    public void Unapprove()
    {
        ApprovedAt = null;
        ApprovedBy = null;
    }
    
    public bool IsWithinShift(DateTimeOffset date)
    {
        return date >= Schedule.Start && date < Schedule.End;
    }

    public void Apply(ShiftCreatedEvent @event)
    {
        Name = @event.Name;
        IsLast = true;
        IsNextToLast = false;
        Schedule = @event.Schedule;
        CreatedAt = @event.CreatedAt;
        WashingMachines = @event.WashingMachines;
    }
    
    public void Apply(LastShiftMovedToNextToLastEvent @event)
    {
        IsLast = false;
        IsNextToLast = true;
    }
    
    public void Apply(NextToLastShiftMovedBackEvent @event)
    {
        IsLast = false;
        IsNextToLast = false;
    }
    
    public void Apply(ShiftApprovedEvent @event)
    {
        ApprovedAt = @event.ApprovedAt;
        ApprovedBy = @event.ApprovedBy;
    }
    
    public void Apply(ShiftUnapprovedEvent @event)
    {
        Unapprove();
    }
}