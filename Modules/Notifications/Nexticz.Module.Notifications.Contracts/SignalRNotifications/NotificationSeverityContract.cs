using System.Text.Json.Serialization;

namespace Nexticz.Module.Notifications.Contracts.SignalRNotifications;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationSeverityContract
{
    Info,
    Success,
    Warning,
    Error
}