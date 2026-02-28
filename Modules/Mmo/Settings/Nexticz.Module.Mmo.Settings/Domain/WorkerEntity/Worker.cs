using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity.Events;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.WorkerEntity;

public class Worker : Entity
{
    public string Name { get; private set; }
    public int Pin { get; private set; }
    public bool IsActive { get; set; }
    
    // We need private constructor due to Marten deserialization
    private Worker() {}
    
    public Worker(
        string name, 
        int pin,
        bool isActive,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        
        if (pin is >= 4 and <= 8)
            throw new ArgumentException("Pin must be between 4 and 8 digits", nameof(pin));
        
        Name = name;
        Pin = pin;
        IsActive = isActive;
    }

    public void Apply(WorkerCreatedEvent @event)
    {
        Id = @event.Id;
        Name = @event.Name;
        Pin = @event.Pin;
        IsActive = @event.IsActive;
    }

    public void Apply(WorkerUpdatedEvent @event)
    {
        Name = @event.Name;
        Pin = @event.Pin;
        IsActive = @event.IsActive;
    }
}