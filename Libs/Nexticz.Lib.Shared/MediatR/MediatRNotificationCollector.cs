using MediatR;

namespace Nexticz.Lib.Shared.MediatR;

public abstract class MediatRNotificationCollector(IMediator mediator) : IMediatRNotificationCollector
{
    private readonly List<INotification> _notifications = [];
    private bool _isDelayedPublishing = false;

    public void AddNotification(INotification notification)
    {
        _notifications.Add(notification);
    }

    public async Task PublishNotificationsAsync(CancellationToken cancellationToken)
    {
        if (_isDelayedPublishing)
            return;

        var notifications = _notifications.ToArray();
        Clear(); // clear the list to avoid publishing the same notifications multiple times
        
        foreach (var notification in notifications)
        {
            await mediator.Publish(notification, cancellationToken);
        }
    }

    public void Clear()
    {
        _notifications.Clear();
    }

    public IDisposable DelayedNotifications()
    {
        _isDelayedPublishing = true;
        return new DelayedScope(this);
    }

    private void EndDelay()
    {
        _isDelayedPublishing = false;
    }

    private class DelayedScope(MediatRNotificationCollector collector) : IDisposable
    {
        public void Dispose()
        {
            collector.EndDelay();
        }
    }
}