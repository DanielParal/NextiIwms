using Nexticz.Module.Notifications.Domain.SignalRNotificationAggregate;

namespace Nexticz.Module.Notifications.Application.SignalRNotifications;

internal interface ISignalRNotifier
{
    Task BroadcastNotificationAsync(SignalRNotification signalRNotification, CancellationToken cancellationToken);
}