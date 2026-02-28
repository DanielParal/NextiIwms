using MediatR;

namespace Nexticz.Lib.Shared.MediatR;

public interface IMediatRNotificationCollector
{
    void AddNotification(INotification notification);
    Task PublishNotificationsAsync(CancellationToken cancellationToken);
    void Clear();
    IDisposable DelayedNotifications();
}