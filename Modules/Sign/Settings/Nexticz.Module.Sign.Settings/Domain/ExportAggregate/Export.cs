using Nexticz.Module.Sign.Settings.Domain.ExportAggregate.Events;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.ExportAggregate;

public class Export : AggregateRoot
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