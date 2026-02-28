using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Washing.Contracts.WashingMachineSpeeds.Notifications;
using Nexticz.Module.Mmo.Reporting.Application.WashingMachineSpeeds.Commands.ChangeWashingMachineSpeed;
using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.WashingMachineSpeeds.Notifications;

internal class WashingMachineSpeedChangedHandler(
    ILogger<WashingMachineSpeedChangedHandler> logger,
    ISender sender) 
    : INotificationHandler<WashingMachineSpeedChangedNotification>
{
    public async Task Handle(WashingMachineSpeedChangedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Reporting - washing machine speed changed notification received. " +
                              "Code: {Code}, Speed: {Speed}, SpeedLevel: {SpeedLevel}, DateChanges: {DateChanges}.",
            notification.Code, notification.Speed, notification.SpeedLevel, notification.DateChanged);
        
        await sender.Send(new ChangeWashingMachineSpeedCommand(notification.Code, notification.Speed, (SpeedLevel)notification.SpeedLevel, notification.DateChanged), cancellationToken);
    }
} 