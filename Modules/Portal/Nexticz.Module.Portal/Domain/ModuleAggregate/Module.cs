using ErrorOr;
using Nexticz.Module.Portal.Domain.ModuleAggregate.Events;

namespace Nexticz.Module.Portal.Domain.ModuleAggregate;

public class Module : AggregateRoot
{
    public string Name { get; private set; }
    public string Icon { get; private set; }
    public string BaseUrl { get; private set; }
    public bool IsActive { get; private set; }
    public int SortOrder { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Module() {}

    private Module(
        string name, 
        string icon, 
        string baseUrl, 
        bool isActive, 
        int sortOrder, 
        DateTimeOffset createdAt,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Name = name;
        Icon = icon;
        BaseUrl = baseUrl;
        IsActive = isActive;
        SortOrder = sortOrder;
        CreatedAt = createdAt;
    }

    public static ErrorOr<Module> CreateFrom(
        string name, 
        string icon, 
        string baseUrl, 
        bool isActive, 
        int order,
        DateTimeOffset createdAt)
    {
        var validation = IsValid(name, icon, baseUrl);

        if (validation.IsError)
            return validation.Errors;
        
        return new Module(name, icon, baseUrl, isActive, order, createdAt);
    }

    public ErrorOr<Success> Update(string name, string icon, string baseUrl, bool isActive)
    {
        var validation = IsValid(name, icon, baseUrl);
        if (validation.IsError)
            return validation.Errors;
        
        Name = name;
        Icon = icon;
        BaseUrl = baseUrl;
        IsActive = isActive;
        
        return Result.Success;       
    }

    private static ErrorOr<Success> IsValid(string name, string icon, string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ModuleDomainErrors.ValidationNameIsRequired();
        
        if (string.IsNullOrWhiteSpace(icon))
            return ModuleDomainErrors.ValidationIconIsRequired();
        
        if (string.IsNullOrWhiteSpace(baseUrl))
            return ModuleDomainErrors.ValidationBaseUrlIsRequired();
        
        return Result.Success;
    }

    public void Apply(ModuleCreatedEvent @event)
    {
        Name = @event.Name;
        Icon = @event.Icon;
        BaseUrl = @event.BaseUrl;
        IsActive = @event.IsActive;
        SortOrder = @event.SortOrder;
        CreatedAt = @event.CreatedAt;
    }
    
    public void Apply(ModuleUpdatedEvent @event)
    {
        Name = @event.Name;
        Icon = @event.Icon;
        BaseUrl = @event.BaseUrl;
        IsActive = @event.IsActive;
    }
    
    public void Apply(ModuleOrderChangedEvent @event)
    {
        SortOrder = @event.NewSortOrder;
    }
}