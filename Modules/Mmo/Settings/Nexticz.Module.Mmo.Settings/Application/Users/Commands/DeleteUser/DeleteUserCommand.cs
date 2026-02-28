using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Application.Users.Commands.DeleteUser;

internal record DeleteUserCommand(string UserName) : ISettingsCommand<ErrorOr<Deleted>>;