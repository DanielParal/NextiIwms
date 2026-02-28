using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.Users.Commands.UpdateUser;

internal record UpdateUserCommand(
    string UserName, 
    string[] DepositorCodes,
    string[] DepositorGroupCodes,
    string[] SigningDeviceCodes) : ISettingsCommand<ErrorOr<Updated>>;