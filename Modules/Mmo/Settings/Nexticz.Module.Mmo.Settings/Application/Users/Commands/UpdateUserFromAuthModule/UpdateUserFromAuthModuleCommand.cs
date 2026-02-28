using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Application.Users.Commands.UpdateUserFromAuthModule;

internal record UpdateUserFromAuthModuleCommand(string UserName, string[] Roles, string[] Permissions) : ISettingsCommand<ErrorOr<Updated>>;