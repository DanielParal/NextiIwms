using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.Users.Commands.UpdateUserFromAuthModule;

internal record UpdateUserFromAuthModuleCommand(string UserName, string? FullName, string[] Roles, string[] Permissions) 
    : ISettingsCommand<ErrorOr<Updated>>;