using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Notifications.Application.SignalRNotifications;

internal abstract class SignalRNotificationErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "notification-service-signalRNotification-";
    
    public static Error ValidationSignalRFailedToSend => Error.Validation(
        ComponentSlug + "ValidationSignalRFailedToSend",
        "SignalR zprávu se nepodařilo odeslat."
    );
}