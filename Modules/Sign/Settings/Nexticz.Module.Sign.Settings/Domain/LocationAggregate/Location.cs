using Nexticz.Module.Sign.Settings.Domain.LocationAggregate.Events;
using Nexticz.Module.Sign.SharedKernel;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.LocationAggregate;

public class Location : AggregateRoot
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Location() {}
    
    public Location(
        string code, 
        string name, 
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        Name = name;
    }

    public void Apply(LocationCreatedEvent @event)
    {
        Code = @event.Code;
        Name = @event.Name;
    }
    
    public void Apply(LocationUpdatedEvent @event)
    {
        Name = @event.Name;
    }
}