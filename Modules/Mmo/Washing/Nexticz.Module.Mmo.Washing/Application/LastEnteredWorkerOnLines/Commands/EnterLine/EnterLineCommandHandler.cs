using MediatR;
using ErrorOr;
using Marten.Exceptions;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Workers.Queries;
using Nexticz.Module.Mmo.Washing.Contracts.LastEnteredWorkerOnLines.Notifications;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Washing.Domain.LastEnteredWorkerOnLineAggregate;
using Nexticz.Module.Mmo.Washing.Domain.LastEnteredWorkerOnLineAggregate.Events;
using Nexticz.Module.Mmo.Washing.Domain.WorkerEntity;

namespace Nexticz.Module.Mmo.Washing.Application.LastEnteredWorkerOnLines.Commands.EnterLine;

internal class EnterLineCommandHandler(
    ISender sender,
    ILogger<EnterLineCommandHandler> logger,
    IWashingUnitOfWork washingUnitOfWork,
    IWashingReadOnlyEventStoreRepository readOnlyEventStoreRepository,
    IWashingNotificationCollector notificationCollector,
    IClock clock) 
    : IRequestHandler<EnterLineCommand, ErrorOr<Worker>>
{
    public async Task<ErrorOr<Worker>> Handle(EnterLineCommand request, CancellationToken cancellationToken)
    {
        var workerFromSettings =
            await sender.Send(new GetWorkerResponseByPinQuery(request.WorkerPin), cancellationToken);

        if (workerFromSettings.IsError)
        {
            logger.LogInformation("Washing - worker was not found in settings by pin. Cannot enter line.");
            return LastEnteredWorkerOnLineErrors.ValidationWorkerNotFound;
        }

        if (!workerFromSettings.Value.IsActive)
        {
            logger.LogInformation("Washing - worker is not active. Cannot enter line. WorkerId: {WorkerId}.", 
                workerFromSettings.Value.Id);;
            return LastEnteredWorkerOnLineErrors.ValidationWorkerIsNotActive;
        }
        
        var enteredAt = clock.UtcNowOffset;
        var worker = new Worker(workerFromSettings.Value.Name, workerFromSettings.Value.Id);
        var lastEnteredWorkerOnLine = new LastEnteredWorkerOnLine(request.LineCode.ToUpperInvariant(), worker, enteredAt);
        var workerEnteredLineEvent = new WorkerEnteredLineEvent(
            lastEnteredWorkerOnLine.Id, lastEnteredWorkerOnLine.LineCode, lastEnteredWorkerOnLine.Worker!.Id, 
            lastEnteredWorkerOnLine.Worker.Name, enteredAt);

        if (await readOnlyEventStoreRepository.DoesStreamExistAsync(lastEnteredWorkerOnLine.Id,
                cancellationToken))
        {
            washingUnitOfWork.AppendEvent(lastEnteredWorkerOnLine.Id, workerEnteredLineEvent);
        }
        else
        {
            washingUnitOfWork.StartStream<WorkerEnteredLineEvent, LastEnteredWorkerOnLine>(lastEnteredWorkerOnLine.Id, workerEnteredLineEvent);
        }
        
        notificationCollector.AddNotification(
            new WorkerEnteredLineNotification(
                lastEnteredWorkerOnLine.Id, lastEnteredWorkerOnLine.LineCode, lastEnteredWorkerOnLine.Worker!.Id, 
                lastEnteredWorkerOnLine.Worker.Name, enteredAt));
        
        logger.LogInformation("Washing - Worker with id {WorkerId} entered line {LineCode}.", 
            lastEnteredWorkerOnLine.Worker!.Id, lastEnteredWorkerOnLine.LineCode);

        return worker;
    }
}