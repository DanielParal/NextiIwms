using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Users;

public record UserResponse(
    [property: Required] Guid Id,
    [property: Required] string UserName,
    string? FullName,
    [property: Required] bool IsActive,
    [property: Required] RoleContract[] Roles,
    [property: Required] PermissionContract[] Permissions,
    [property: Required] string[] DepositorCodes,
    [property: Required] string[] DepositorGroupCodes,
    [property: Required] string[] SigningDeviceCodes,
    [property: Required] bool HasSignatureFile
    );