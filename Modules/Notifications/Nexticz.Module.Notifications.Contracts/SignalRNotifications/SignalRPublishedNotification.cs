namespace Nexticz.Module.Notifications.Contracts.SignalRNotifications;

public record SignalRPublishedNotification(
    string Title,
    string Description,
    string? Data,
    string NotificationType,
    string ModuleName,
    NotificationSeverityContract Severity,
    string[] Receivers);