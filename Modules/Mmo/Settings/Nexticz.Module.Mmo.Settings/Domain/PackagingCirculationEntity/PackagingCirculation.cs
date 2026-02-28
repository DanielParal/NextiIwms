using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity.Events;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;

public class PackagingCirculation : Entity
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private PackagingCirculation() {}
    
    public PackagingCirculation(string code, string name, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        Name = name;
    }

    public void Apply(PackagingCirculationCreatedEvent @event)
    {
        Id = @event.Id;
        Code = @event.Code;
        Name = @event.Name;
    }

    public void Apply(PackagingCirculationNameUpdatedEvent @event)
    {
        Name = @event.Name;
    }
}