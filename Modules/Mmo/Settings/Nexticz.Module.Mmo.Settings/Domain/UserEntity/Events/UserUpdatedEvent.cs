using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.UserEntity.Events;

public record UserUpdatedEvent(Guid Id, ReceivableNotification[] ReceivableNotifications) : IMartenEvent;