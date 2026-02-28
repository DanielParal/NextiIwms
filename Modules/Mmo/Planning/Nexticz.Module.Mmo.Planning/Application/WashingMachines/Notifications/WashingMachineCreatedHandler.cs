using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Notifications;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateWashingMachine;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Notifications;

internal class WashingMachineCreatedHandler(
    ILogger<WashingMachineCreatedHandler> logger,
    ISender sender) 
    : INotificationHandler<WashingMachineCreatedNotification>
{
    public async Task Handle(WashingMachineCreatedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Washing machine created notification received. Washing machine code: {WashingMachineCode}.",
            notification.WashingMachineCode);
        
        await sender.Send(
            new CreateWashingMachineCommand(
                notification.WashingMachineCode, 
                (WashingMachineStatus)notification.Status,
                notification.WashingMachineLines.Select(x => new LineQueue(x.Code, x.IsActive, [])).ToArray()), 
            cancellationToken);
    }

}