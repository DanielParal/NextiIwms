using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate.Events;
using Nexticz.Module.Sign.SharedKernel;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.PartnerAggregate;

public class Partner : AggregateRoot
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Partner() {}
    
    public Partner(
        string code, 
        string name, 
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        Name = name;
    }

    public void Apply(PartnerCreatedEvent @event)
    {
        Code = @event.Code;
        Name = @event.Name;
    }
    
    public void Apply(PartnerUpdatedEvent @event)
    {
        Name = @event.Name;
    }
}