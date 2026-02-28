using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Notifications;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.UpdateWashingMachineStatus;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Notifications;

internal class WashingMachineStatusUpdatedHandler(
    ILogger<WashingMachineStatusUpdatedHandler> logger,
    ISender sender) 
    : INotificationHandler<WashingMachineStatusUpdatedNotification>
{
    public async Task Handle(WashingMachineStatusUpdatedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Washing machine status updated notification received. Washing machine code: {WashingMachineCode}.",
            notification.WashingMachineCode);

        await sender.Send(
            new UpdateWashingMachineStatusCommand(
                notification.WashingMachineCode, 
                (WashingMachineStatus)notification.Status,
                notification.WashingMachineLines.Select(x => new LineQueue(x.Code, x.IsActive, [])).ToArray()), 
            cancellationToken);
    }
}