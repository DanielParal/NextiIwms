namespace Nexticz.Module.Notifications.Domain.SignalRNotificationAggregate.Events;

internal record SignalRNotificationSentEvent(
    Guid Id, string Title, string Message, string? Data, 
    string NotificationType, string ModuleName, NotificationSeverity Severity, string[] Receivers);