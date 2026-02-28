using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Users;

public record UpdateUserRequest(
    [property: Required] string[] DepositorCodes,
    [property: Required] string[] DepositorGroupCodes,
    [property: Required] string[] SigningDeviceCodes,
    string? EmailSignature);