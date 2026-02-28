using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate.Events;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;

public class WashingMachineSpeed : AggregateRoot
{
    public string Code { get; private set; }
    public int Speed { get; private set; }
    public SpeedLevel SpeedLevel { get; private set; }
    public DateTimeOffset DateStarted { get; private set; }
    public DateTimeOffset? DateEnded { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private WashingMachineSpeed() {}

    public WashingMachineSpeed(
        string code,
        int speed,
        SpeedLevel speedLevel,
        DateTimeOffset dateStarted,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        Code = code;
        Speed = speed;
        SpeedLevel = speedLevel;
        DateStarted = dateStarted;
        DateEnded = null;
    }

    public void Apply(WashingMachineSpeedStartedEvent @event)
    {
        Id = @event.Id;
        Code = @event.Code;
        Speed = @event.Speed;
        SpeedLevel = @event.SpeedLevel;
        DateStarted = @event.DateStarted;
    }
    
    public void Apply(WashingMachineSpeedEndedEvent @event)
    {
        DateEnded = @event.DateEnded;
    }
}