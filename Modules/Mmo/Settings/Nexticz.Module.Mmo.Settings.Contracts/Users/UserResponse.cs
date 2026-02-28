using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Users;

public record UserResponse(
    [property: Required] Guid Id,
    [property: Required] string UserName,
    [property: Required] bool IsActive,
    [property: Required] RoleContract[] Roles,
    [property: Required] PermissionContract[] Permissions,
    [property: Required] ReceivableNotificationContract[] ReceivableNotifications);