using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate.Events;
using Nexticz.Module.Sign.SharedKernel;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate;

public class DepositorGroup : AggregateRoot
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private DepositorGroup() {}
    
    public DepositorGroup(
        string code, 
        string name, 
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        Name = name;
    }

    public void Apply(DepositorGroupCreatedEvent @event)
    {
        Code = @event.Code;
        Name = @event.Name;
    }
    
    public void Apply(DepositorGroupUpdatedEvent @event)
    {
        Name = @event.Name;
    }
}