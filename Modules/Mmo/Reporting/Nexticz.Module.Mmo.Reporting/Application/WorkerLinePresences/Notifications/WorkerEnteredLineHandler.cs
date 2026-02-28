using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Reporting.Application.Shifts;
using Nexticz.Module.Mmo.Washing.Contracts.LastEnteredWorkerOnLines.Notifications;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Orchestrators;
using Nexticz.Module.Mmo.Reporting.Application.WorkerLinePresences.Commands.EnterLine;

namespace Nexticz.Module.Mmo.Reporting.Application.WorkerLinePresences.Notifications;

internal class WorkerEnteredLineHandler(
    ISender sender,
    ILogger<WorkerEnteredLineHandler> logger,
    IShiftCreationOrchestrator shiftCreationOrchestrator) : INotificationHandler<WorkerEnteredLineNotification>
{
    public async Task Handle(WorkerEnteredLineNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Reporting - worker entered line notification received. " +
                              "WorkerId: {WorkerId}, LineCode: {LineCode}, DateEntered: {DateEntered}.",
            notification.WorkerId, notification.LineCode, notification.EnteredAt);
        
        var shift = await shiftCreationOrchestrator.EnsureCurrentShiftAsync(cancellationToken);

        await sender.Send(new EnterLineCommand(
                shift.Id, notification.LineCode, notification.WorkerId, notification.WorkerName, notification.EnteredAt),
            cancellationToken);
    }
}