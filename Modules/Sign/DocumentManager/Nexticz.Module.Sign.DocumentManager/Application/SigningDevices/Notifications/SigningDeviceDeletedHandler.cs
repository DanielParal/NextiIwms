using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.SigningDevices.Notifications;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.DeleteSigningDevice;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Notifications;

internal class SigningDeviceDeletedHandler(
    ILogger<SigningDeviceDeletedHandler> logger,
    ISender sender) : INotificationHandler<SigningDeviceDeletedNotification>
{
    public async Task Handle(SigningDeviceDeletedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("SIGN - DocumentManager - signing device deleted notification received. " +
                              "Code: {SigningDeviceCode}.",
            notification.Code);
        
        await sender.Send(new DeleteSigningDeviceCommand(notification.Code),
            cancellationToken);
    }
}