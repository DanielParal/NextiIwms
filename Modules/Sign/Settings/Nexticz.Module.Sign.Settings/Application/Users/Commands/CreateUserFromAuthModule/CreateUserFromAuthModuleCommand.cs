using ErrorOr;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Users.Commands.CreateUserFromAuthModule;

internal record CreateUserFromAuthModuleCommand(string UserName, string? FullName, string[] Roles, string[] Permissions) 
    : ISettingsCommand<ErrorOr<User>>;