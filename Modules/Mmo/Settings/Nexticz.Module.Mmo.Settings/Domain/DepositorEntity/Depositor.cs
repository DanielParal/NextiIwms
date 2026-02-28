using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity.Events;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.DepositorEntity;

public class Depositor : Entity
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string? BarcodeTemplate { get; private set; }

    // We need private constructor due to Marten deserialization
    private Depositor() {}
    
    public Depositor(
        string code, 
        string name, 
        string? barcodeTemplate,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        Name = name;
        BarcodeTemplate = barcodeTemplate;
    }
    
    public void Apply(DepositorCreatedEvent @event)
    {
        Id = @event.Id;
        Code = @event.Code;
        Name = @event.Name;
        BarcodeTemplate = @event.BarcodeTemplate;
    }
    
    public void Apply(DepositorUpdatedEvent @event)
    {
        Name = @event.Name;
        BarcodeTemplate = @event.BarcodeTemplate;
    }
}