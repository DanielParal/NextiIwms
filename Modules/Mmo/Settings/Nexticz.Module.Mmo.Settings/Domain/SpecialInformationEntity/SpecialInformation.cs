using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity.Events;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;

public class SpecialInformation : Entity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool HasFile { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private SpecialInformation() {}

    public SpecialInformation(
        string title,
        string description,
        bool hasFile,
        Guid? id = null
    ) : base(id ?? Guid.NewGuid())
    {
        Title = title;
        Description = description;
        HasFile = hasFile;
    }
    
    public void Apply(SpecialInformationCreatedEvent @event)
    {
        Title = @event.Title;
        Description = @event.Description;
        HasFile = false;
    }
    
    public void Apply(SpecialInformationUpdatedEvent @event)
    {
        Title = @event.Title;
        Description = @event.Description;
    }
    
    public void Apply(SpecialInformationImageUploadedEvent @event)
    {
        HasFile = true;
    }
    
    public void Apply(SpecialInformationImageDeletedEvent @event)
    {
        HasFile = false;
    }
}