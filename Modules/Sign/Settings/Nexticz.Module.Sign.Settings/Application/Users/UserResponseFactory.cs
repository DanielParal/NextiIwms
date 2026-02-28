using Nexticz.Module.Sign.Settings.Contracts.Users;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Users;

internal static class UserResponseFactory
{
    public static UserResponse Create(User user)
    {
        var roles = user.Roles.Select(Enum.Parse<RoleContract>).ToArray();
        var permissions = user.Permissions.Select(Enum.Parse<PermissionContract>).ToArray();

        return new UserResponse(
            user.Id,
            user.UserName,
            user.FullName,
            user.IsActive,
            roles,
            permissions,
            user.DepositorCodes,
            user.DepositorGroupCodes,
            user.SigningDeviceCodes,
            user.HasSignatureFile);
    }
}