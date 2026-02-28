using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Notifications.Contracts.SignalRNotifications;

public record SignalRNotificationResponse(
    [property: Required] Guid Id,
    [property: Required] string Title,
    [property: Required] string Description,
    string? Data,
    [property: Required] string NotificationType,
    [property: Required] string ModuleName,
    [property: Required] NotificationSeverityContract Severity,
    [property: Required] DateTimeOffset SentAt);