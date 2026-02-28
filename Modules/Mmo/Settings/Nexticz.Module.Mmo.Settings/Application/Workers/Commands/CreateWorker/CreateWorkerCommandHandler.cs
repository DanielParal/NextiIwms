using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Workers.Queries.GetWorkerByPin;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Workers.Commands.CreateWorker;

internal class CreateWorkerCommandHandler(
    ILogger<CreateWorkerCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender
    ) : IRequestHandler<CreateWorkerCommand, ErrorOr<Worker>>
{
    public async Task<ErrorOr<Worker>> Handle(CreateWorkerCommand request, CancellationToken cancellationToken)
    {
        var workerWithPin = await sender.Send(new GetWorkerByPinQuery(request.Pin), cancellationToken);

        if (!workerWithPin.IsError)
        {
            logger.LogWarning("Object {ObjectName} with PIN: {Pin} already exists. Nothing to create.", nameof(Worker), request.Pin);
            return WorkerErrors.ValidationWorkerWithPinAlreadyExist;
        }

        var worker = new Worker(request.Name, request.Pin, request.IsActive);
        var workerCreatedEvent = new WorkerCreatedEvent(
            worker.Id, worker.Name, worker.Pin, worker.IsActive);
        
        unitOfWork
            .StartStream<WorkerCreatedEvent, Worker>(
                worker.Id, workerCreatedEvent);
        
        return worker;
    }
}