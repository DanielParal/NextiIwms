using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity.Events;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity;

public class KitType : Entity
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private KitType() {}
    
    public KitType(string code, string name, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        Name = name;
    }

    public void Apply(KitTypeCreatedEvent @event)
    {
        Id = @event.Id;
        Code = @event.Code;
        Name = @event.Name;
    }

    public void Apply(KitTypeNameUpdatedEvent @event)
    {
        Name = @event.Name;
    }
}