using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity.Events;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;

public class Constant : Entity
{
    public string Key { get; private set; }
    public string Value { get; private set; }
    public ConstantType Type { get; private set; }
    public string? Description { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Constant() {}
    
    public Constant(
        string key, 
        string value, 
        ConstantType type,
        string? description = null,
        Guid? id = null) 
        : base(id ?? Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace", nameof(key));
        
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace", nameof(value));
        
        if (!ConstantValidator.IsValidValue(value, type))
            throw new ArgumentException("Value is not valid for type", nameof(value));
        
        Key = key;
        Value = value;
        Type = type;
        Description = description;
    }

    public void Apply(ConstantCreatedEvent @event)
    {
        Id = @event.Id;
        Key = @event.Key;
        Value = @event.Value;
        Type = @event.Type;
        Description = @event.Description;
    }

    public void Apply(ConstantUpdatedEvent @event)
    {
        Value = @event.Value;
        Description = @event.Description;
    }
    
}