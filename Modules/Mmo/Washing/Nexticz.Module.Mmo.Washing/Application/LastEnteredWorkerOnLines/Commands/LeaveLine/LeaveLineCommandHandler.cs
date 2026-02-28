using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Washing.Contracts.LastEnteredWorkerOnLines.Notifications;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Application.LastEnteredWorkerOnLines.Queries.GetLastWorkerByLineCode;
using Nexticz.Module.Mmo.Washing.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Washing.Domain.LastEnteredWorkerOnLineAggregate.Events;

namespace Nexticz.Module.Mmo.Washing.Application.LastEnteredWorkerOnLines.Commands.LeaveLine;

internal class LeaveLineCommandHandler(
    ISender sender,
    ILogger<LeaveLineCommandHandler> logger,
    IWashingUnitOfWork unitOfWork,
    IWashingNotificationCollector notificationCollector,
    IClock clock
    ) 
    : IRequestHandler<LeaveLineCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(LeaveLineCommand request, CancellationToken cancellationToken)
    {
        var lastWorkerOnLine = await sender.Send(new GetLastWorkerByLineCodeQuery(request.LineCode), cancellationToken);
        if (lastWorkerOnLine.IsError)
        {
            logger.LogInformation("Washing - there is no record in projection for the line line: {LineCode}. There is no-one to leave the line.",
                request.LineCode);
            return LastEnteredWorkerOnLineErrors.ValidationNoUserOnTheLine;
        }

        if (lastWorkerOnLine.Value.Worker is null)
        {
            logger.LogInformation("Washing - there is no user on the line line: {LineCode}. There is no-one to leave the line.",
                request.LineCode);
            return LastEnteredWorkerOnLineErrors.ValidationNoUserOnTheLine;
        }
        
        var leftAt = clock.UtcNowOffset;
        var workerLeftLine = new WorkerLeftLineEvent(
            lastWorkerOnLine.Value.Id, lastWorkerOnLine.Value.LineCode, lastWorkerOnLine.Value.Worker.Id, 
            lastWorkerOnLine.Value.Worker.Name, leftAt);
        unitOfWork.AppendEvent(lastWorkerOnLine.Value.Id, workerLeftLine);
        
        notificationCollector.AddNotification(
            new WorkerLeftLineNotification(
                lastWorkerOnLine.Value.Id, lastWorkerOnLine.Value.LineCode, lastWorkerOnLine.Value.Worker!.Id, 
                lastWorkerOnLine.Value.Worker.Name, leftAt));
        
        logger.LogInformation("Washing - Worker with id {WorkerId} left line {LineCode}.", 
            lastWorkerOnLine.Value.Worker!.Id, lastWorkerOnLine.Value.LineCode);
        
        return Result.Success;
    }
}