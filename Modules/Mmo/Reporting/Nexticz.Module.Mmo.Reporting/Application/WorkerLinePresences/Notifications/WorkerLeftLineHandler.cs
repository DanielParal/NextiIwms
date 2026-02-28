using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Reporting.Application.Shifts;
using Nexticz.Module.Mmo.Washing.Contracts.LastEnteredWorkerOnLines.Notifications;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Orchestrators;
using Nexticz.Module.Mmo.Reporting.Application.WorkerLinePresences.Commands.LeaveLine;

namespace Nexticz.Module.Mmo.Reporting.Application.WorkerLinePresences.Notifications;

internal class WorkerLeftLineHandler(
    ISender sender,
    ILogger<WorkerLeftLineHandler> logger,
    IShiftCreationOrchestrator shiftCreationOrchestrator) 
    : INotificationHandler<WorkerLeftLineNotification>
{
    public async Task Handle(WorkerLeftLineNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Reporting - worker left line notification received. " +
                              "WorkerId: {WorkerId}, LineCode: {LineCode}, DateLeft: {DateLeft}.",
            notification.WorkerId, notification.LineCode, notification.LeftAt);
        
        var shift = await shiftCreationOrchestrator.EnsureCurrentShiftAsync(cancellationToken);

        await sender.Send(new LeaveLineCommand(
                shift.Id, notification.LineCode, notification.WorkerId, notification.WorkerName, notification.LeftAt),
            cancellationToken);
    }
}