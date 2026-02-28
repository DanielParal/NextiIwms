using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.Users.Commands.DeleteUserFromAuthModule;

internal record DeleteUserFromAuthModuleCommand(string UserName) : ISettingsCommand<ErrorOr<Deleted>>;