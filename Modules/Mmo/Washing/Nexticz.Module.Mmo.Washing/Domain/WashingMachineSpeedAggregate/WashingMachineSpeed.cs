using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate.Events;

namespace Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

public class WashingMachineSpeed : AggregateRoot
{
    public string Code { get; private set; }
    public int Speed { get; private set; }
    public SpeedLevel SpeedLevel { get; private set; }
    
    private WashingMachineSpeed() {}
    
    public WashingMachineSpeed(
        string code,
        int speed,
        SpeedLevel speedLevel,
        Guid? id = null) : base(id ?? GenerateIdFromString($"WashingMachineSpeed_{code}"))
    {
        Code = code;
        Speed = speed;
        SpeedLevel = speedLevel;
    }

    public void Apply(WashingMachineSpeedCreatedEvent @event)
    {
        Code = @event.Code;
        Speed = @event.Speed;
        SpeedLevel = @event.SpeedLevel;
    }
    
    public void Apply(WashingMachineSpeedUpdatedEvent @event)
    {
        Code = @event.Code;
        Speed = @event.Speed;
        SpeedLevel = @event.SpeedLevel;
    }
}