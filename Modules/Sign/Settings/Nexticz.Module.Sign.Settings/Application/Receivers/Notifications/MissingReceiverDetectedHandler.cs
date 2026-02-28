using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.DocumentManager.Contracts.PartnersAndReceivers;
using Nexticz.Module.Sign.Settings.Application.Receivers.Commands.CreateReceiver;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.Notifications;

internal class MissingReceiverDetectedHandler(
    ISender sender,
    ILogger<MissingReceiverDetectedNotification> logger) : INotificationHandler<MissingReceiverDetectedNotification>
{
    public async Task Handle(MissingReceiverDetectedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Sign - Settings - {NotificationName} received. Receiver code: {ReceiverCode}, partner code: {PartnerCode}, receiver name: {ReceiverName}.",
            nameof(MissingReceiverDetectedNotification), notification.ReceiverCode, notification.PartnerCode, notification.Name);
        
        await sender.Send(new CreateReceiverCommand(notification.ReceiverCode, notification.PartnerCode, notification.Name), cancellationToken);
    }
}