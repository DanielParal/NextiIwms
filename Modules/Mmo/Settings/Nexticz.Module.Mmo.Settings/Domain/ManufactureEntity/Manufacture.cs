using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity.Events;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;

public class Manufacture : Entity
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Manufacture() {}
    
    public Manufacture(string code, string name, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        Name = name;
    }

    public void Apply(ManufactureCreatedEvent @event)
    {
        Id = @event.Id;
        Code = @event.Code;
        Name = @event.Name;
    }

    public void Apply(ManufactureNameUpdatedEvent @event)
    {
        Name = @event.Name;
    }
}