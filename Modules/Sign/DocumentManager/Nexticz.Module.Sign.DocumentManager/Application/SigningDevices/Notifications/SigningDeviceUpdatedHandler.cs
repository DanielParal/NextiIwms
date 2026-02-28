using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.SigningDevices.Notifications;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.UpdateSigningDevice;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Notifications;

internal class SigningDeviceUpdatedHandler(
    ILogger<SigningDeviceUpdatedHandler> logger,
    ISender sender) : INotificationHandler<SigningDeviceUpdatedNotification>
{
    public async Task Handle(SigningDeviceUpdatedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("SIGN - DocumentManager - signing device updated notification received. " +
                              "Code: {SigningDeviceCode}, Name: {SigningDeviceName}, " +
                              "IsActive: {SigningDeviceIsActive}, PrinterCode: {PrinterCode}.",
            notification.Code, notification.Name, notification.IsActive, notification.PrinterCode);
        
        await sender.Send(new UpdateSigningDeviceCommand(notification.Code, notification.Name, notification.IsActive, notification.PrinterCode),
            cancellationToken);
    }
}