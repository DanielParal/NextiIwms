using ErrorOr;
using MediatR;
using Nexticz.Module.Notifications.Domain;

namespace Nexticz.Module.Notifications.Application.SignalRNotifications.Commands.SendSignalRNotification;

internal record SendSignalRNotificationCommand(
    string Title, string Message, string? Data, string NotificationType,
    string ModuleName, NotificationSeverity Severity, string[] Receivers) 
    : INotificationCommand<ErrorOr<Success>>;