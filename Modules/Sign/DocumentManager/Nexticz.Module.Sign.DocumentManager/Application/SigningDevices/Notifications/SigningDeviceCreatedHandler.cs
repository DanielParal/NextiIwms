using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.SigningDevices.Notifications;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.CreateSigningDevice;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Notifications;

internal class SigningDeviceCreatedHandler(
    ILogger<SigningDeviceCreatedHandler> logger,
    ISender sender) : INotificationHandler<SigningDeviceCreatedNotification>
{
    public async Task Handle(SigningDeviceCreatedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("SIGN - DocumentManager - signing device created notification received. " +
                              "Code: {SigningDeviceCode}, Name: {SigningDeviceName}, " +
                              "IsActive: {SigningDeviceIsActive}, PrinterCode: {SigningDevicePrinterCode}.",
            notification.Code, notification.Name, notification.IsActive, notification.PrinterCode);

        await sender.Send(new CreateSigningDeviceCommand(notification.Code, notification.Name, notification.IsActive, notification.PrinterCode),
            cancellationToken);
    }
}