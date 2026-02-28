using Nexticz.Lib.Shared.DomainCore;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Domain.UserAggregate;

public class User : AggregateRoot
{
    public string UserName { get; private set; }
    public string? FullName { get; private set; }
    public bool IsActive { get; private set; }
    public string[] Roles { get; private set; }
    public string[] Permissions { get; private set; }
    public string[] DepositorCodes { get; private set; }
    public string[] DepositorGroupCodes { get; private set; }
    public string[] SigningDeviceCodes { get; private set; }
    public bool HasSignatureFile { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private User() {}
    
    public User(
        string userName,
        string? fullName,
        bool isActive,
        string[] roles,
        string[] permissions,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("UserName cannot be null or empty", nameof(UserName));
        
        UserName = userName;
        FullName = fullName;
        IsActive = isActive;
        Roles = roles;
        Permissions = permissions;
    }

    public void Apply(UserCreatedFromAuthModuleEvent @event)
    {
        UserName = @event.UserName;
        FullName = @event.FullName;
        IsActive = true;
        Roles = @event.Roles;
        Permissions = @event.Permissions;
        DepositorCodes = [];
        DepositorGroupCodes = [];
        SigningDeviceCodes = [];
        HasSignatureFile = false;
    }
    
    public void Apply(UserUpdatedFromAuthModuleEvent @event)
    {
        FullName = @event.FullName;
        IsActive = @event.IsActive;
        Roles = @event.Roles;
        Permissions = @event.Permissions;
    }
    
    public void Apply(UserUpdatedEvent @event)
    {
        DepositorCodes = @event.DepositorCodes;
        DepositorGroupCodes = @event.DepositorGroupCodes;
        SigningDeviceCodes = @event.SigningDeviceCodes;
    }
    
    public void Apply(UserSignatureUploadedEvent @event)
    {
        HasSignatureFile = true;
    }
    
    public void Apply(UserSignatureDeletedEvent @event)
    {
        HasSignatureFile = false;
    }
}