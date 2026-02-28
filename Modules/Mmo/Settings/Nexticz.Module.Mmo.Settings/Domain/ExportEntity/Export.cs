using Nexticz.Module.Mmo.Settings.Domain.ExportEntity.Events;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.ExportEntity;

public class Export : Entity
{
    public string UserName { get; private set; }
    public ExportType Type { get; private set; }
    public DateTimeOffset DateCreated { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Export() {}
    
    public Export(
        string userName,
        ExportType type,
        DateTimeOffset dateCreated,
        Guid? id = null) 
        : base(id ?? Guid.NewGuid())
    {
        UserName = userName;
        Type = type;
        DateCreated = dateCreated;
    }
    
    public void Apply(ExportCreatedEvent @event)
    {
        Id = @event.Id;
        Type = @event.Type;
        UserName = @event.UserName;
        DateCreated = @event.DateCreated;
    }
}