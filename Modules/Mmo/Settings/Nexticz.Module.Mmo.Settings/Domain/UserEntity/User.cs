using Nexticz.Module.Mmo.Settings.Domain.UserEntity.Events;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.UserEntity;

public class User : Entity
{
    public string UserName { get; private set; }
    public bool IsActive { get; private set; }
    public string[] Roles { get; private set; }
    public string[] Permissions { get; private set; }
    public ReceivableNotification[] ReceivableNotifications { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private User() {}
    
    public User(
        string userName,
        bool isActive,
        string[] roles,
        string[] permissions,
        ReceivableNotification[] receivableNotifications,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("UserName cannot be null or empty", nameof(UserName));
        
        UserName = userName;
        IsActive = isActive;
        Roles = roles;
        Permissions = permissions;
        ReceivableNotifications = receivableNotifications;
    }

    public void Apply(UserCreatedEvent @event)
    {
        Id = @event.Id;
        UserName = @event.UserName;
        IsActive = @event.IsActive;
        Roles = @event.Roles;
        Permissions = @event.Permissions;
        ReceivableNotifications = @event.ReceivableNotifications;
    }
    
    public void Apply(UserUpdatedFromAuthModuleEvent @event)
    {
        IsActive = @event.IsActive;
        Roles = @event.Roles;
        Permissions = @event.Permissions;
    }
    
    public void Apply(UserUpdatedEvent @event)
    {
        ReceivableNotifications = @event.ReceivableNotifications;
    }
}