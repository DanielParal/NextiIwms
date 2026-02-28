using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate.Events;

namespace Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate;

public class WashingMachineSos : AggregateRoot
{
    public string Code { get; private set; }
    public bool IsHelpNeeded { get; private set; }
    
    private WashingMachineSos() {}
    
    public WashingMachineSos(
        string code,
        bool isHelpNeeded,
        Guid? id = null) : base(id ?? GenerateIdFromString($"WashingMachineSos_{code}"))
    {
        Code = code;
        IsHelpNeeded = isHelpNeeded;
    }

    public void Apply(WashingMachineSosCalledEvent @event)
    {
        Code = @event.Code;
        IsHelpNeeded = true;
    }
    
    public void Apply(WashingMachineSosResolvedEvent @event)
    {
        IsHelpNeeded = false;
    }
}