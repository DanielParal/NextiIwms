using Nexticz.Module.Mmo.Settings.Contracts.Users;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Users;

internal static class UserResponseFactory
{
    public static UserResponse Create(User user)
    {
        var roles = user.Roles.Select(Enum.Parse<RoleContract>).ToArray();
        var permissions = user.Permissions.Select(Enum.Parse<PermissionContract>).ToArray();
        var receivableNotification = user.ReceivableNotifications
            .Select(s => Enum.Parse<ReceivableNotificationContract>(s.ToString()))
            .ToArray();
        return new UserResponse(user.Id, user.UserName, user.IsActive, roles, permissions, receivableNotification);
    }
}