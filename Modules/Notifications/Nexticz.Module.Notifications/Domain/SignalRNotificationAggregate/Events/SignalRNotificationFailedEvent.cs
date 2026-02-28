namespace Nexticz.Module.Notifications.Domain.SignalRNotificationAggregate.Events;

internal record SignalRNotificationFailedEvent(
    Guid Id, string Title, string Message, string? Data, 
    string NotificationType, string ModuleName, NotificationSeverity Severity,
    string[] Receivers, string[] FailureReasons);