using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate.Events;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;

public class Receiver : AggregateRoot
{
    public string Code { get; private set; }
    public string PartnerCode { get; private set; }
    public string Name { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Receiver() {}
    
    public Receiver(
        string code, 
        string partnerCode,
        string name, 
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        PartnerCode = partnerCode;
        Name = name;
    }

    public void Apply(ReceiverCreatedEvent @event)
    {
        Code = @event.Code;
        PartnerCode = @event.PartnerCode;
        Name = @event.Name;
    }
    
    public void Apply(ReceiverUpdatedEvent @event)
    {
        Name = @event.Name;
        PartnerCode = @event.PartnerCode;
    }
}