namespace Nexticz.Module.Notifications.Domain.SignalRNotificationAggregate;

public class SignalRNotification : Entity
{
    public string Title { get; private set; }
    public string Message { get; private set; }
    public string? Data { get; private set; }
    public string NotificationType { get; private set; }
    public string ModuleName { get; private set; }
    public NotificationSeverity Severity { get; private set; }
    public string[] Receivers { get; private set; }

    // We need private constructor due to Marten deserialization
    private SignalRNotification() {}

    public SignalRNotification(
        string title,
        string message,
        string? data,
        string notificationType,
        string moduleName,
        NotificationSeverity severity,
        string[] receivers,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        Title = title;
        Message = message;
        Data = data;
        NotificationType = notificationType;
        ModuleName = moduleName;
        Severity = severity;
        Receivers = receivers;
    }
}