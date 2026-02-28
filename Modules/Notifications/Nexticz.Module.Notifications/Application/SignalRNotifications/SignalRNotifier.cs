using Microsoft.AspNetCore.SignalR;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Notifications.Contracts.OpenApiContracts;
using Nexticz.Module.Notifications.Contracts.SignalRNotifications;
using Nexticz.Module.Notifications.Domain.SignalRNotificationAggregate;

namespace Nexticz.Module.Notifications.Application.SignalRNotifications;

internal class SignalRNotifier(
    IHubContext<SignalRHub> signalRHubContext,
    IClock clock) : ISignalRNotifier
{
    private const string SignalRReceiveMethodName = nameof(SignalRReceiveNotificationNameContract.ReceiveNotification);
    
    public async Task BroadcastNotificationAsync(SignalRNotification signalRNotification, CancellationToken cancellationToken)
    {
        foreach (var receiver in signalRNotification.Receivers)
        {
            await signalRHubContext.Clients.Groups(receiver).SendAsync(SignalRReceiveMethodName, 
                new SignalRNotificationResponse(signalRNotification.Id, signalRNotification.Title, signalRNotification.Message, signalRNotification.Data, signalRNotification.NotificationType, signalRNotification.ModuleName, (NotificationSeverityContract)signalRNotification.Severity, clock.UtcNowOffset), 
                cancellationToken);
        }
    }
}