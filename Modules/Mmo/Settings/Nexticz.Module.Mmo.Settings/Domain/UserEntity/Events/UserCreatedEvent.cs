using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.UserEntity.Events;

public record UserCreatedEvent(Guid Id, string UserName, bool IsActive, string[] Roles, string[] Permissions, ReceivableNotification[] ReceivableNotifications) : IMartenEvent;