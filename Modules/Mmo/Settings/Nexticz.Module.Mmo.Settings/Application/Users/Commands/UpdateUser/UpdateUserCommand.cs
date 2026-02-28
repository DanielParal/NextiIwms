using ErrorOr;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Users.Commands.UpdateUser;

internal record UpdateUserCommand(Guid Id, ReceivableNotification[] ReceivableNotifications) : ISettingsCommand<ErrorOr<Updated>>;