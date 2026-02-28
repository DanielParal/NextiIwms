using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity.Events;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;

public class PackagingType : Entity
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private PackagingType() {}
    
    public PackagingType(
        string code,
        string name, 
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        Name = name;
    }
    
    public void Apply(PackagingTypeCreatedEvent @event)
    {
        Id = @event.Id;
        Code = @event.Code;
        Name = @event.Name;
    }
    
    public void Apply(PackagingTypeNameUpdatedEvent @event)
    {
        Name = @event.Name;
    }
}