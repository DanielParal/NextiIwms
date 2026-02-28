using ErrorOr;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Users.Commands.CreateUser;

internal record CreateUserCommand(string UserName, string[] Roles, string[] Permissions) : ISettingsCommand<ErrorOr<User>>;