using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.DocumentManager.Contracts.PartnersAndReceivers;
using Nexticz.Module.Sign.Settings.Application.Partners.Commands.CreatePartner;

namespace Nexticz.Module.Sign.Settings.Application.Partners.Notifications;

internal class MissingPartnerDetectedHandler(
    ISender sender,
    ILogger<MissingPartnerDetectedHandler> logger) : INotificationHandler<MissingPartnerDetectedNotification>
{
    public async Task Handle(MissingPartnerDetectedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Sign - Settings - {NotificationName} received. Partner code: {PartnerCode}, Partner name: {PartnerName}.",
            nameof(MissingPartnerDetectedNotification), notification.Code, notification.Name);
        
        await sender.Send(new CreatePartnerCommand(notification.Code, notification.Name), cancellationToken);
    }
}