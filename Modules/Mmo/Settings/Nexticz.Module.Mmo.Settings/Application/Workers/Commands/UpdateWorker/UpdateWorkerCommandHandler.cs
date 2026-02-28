using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Workers.Queries.GetWorkerById;
using Nexticz.Module.Mmo.Settings.Application.Workers.Queries.GetWorkerByPin;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Workers.Commands.UpdateWorker;

internal class UpdateWorkerCommandHandler(
    ILogger<UpdateWorkerCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender
    ) : IRequestHandler<UpdateWorkerCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateWorkerCommand request, CancellationToken cancellationToken)
    {
        var worker = await sender.Send(new GetWorkerByIdQuery(request.Id), cancellationToken);

        if (worker.IsError)
        {
            logger.LogWarning("Did not find object {ObjectName} with Id: {Id}. Nothing to update", nameof(Constant), request.Id);
            return worker.Errors;
        }

        if (ShouldUpdatePin(worker.Value.Pin, request.Pin))
        {
            var isPinUnique = await IsPinUniqueAsync(request, cancellationToken);
            if(isPinUnique.IsError)
                return isPinUnique.Errors;
        }
        
        var workerUpdatedEvent = new WorkerUpdatedEvent(request.Name, request.Pin, request.IsActive);
        unitOfWork.AppendEvent(worker.Value.Id, workerUpdatedEvent);
        return Result.Updated;
    }

    private async Task<ErrorOr<Success>> IsPinUniqueAsync(UpdateWorkerCommand request, CancellationToken cancellationToken)
    {
        var workerWithPin = await sender.Send(new GetWorkerByPinQuery(request.Pin), cancellationToken);
        
        if (!workerWithPin.IsError)
        {
            logger.LogWarning("Object {ObjectName} with PIN: {Pin} already exists. We cannot update object with Id: {Id}. Nothing to update.", 
                nameof(Worker), request.Pin, request.Id);
            return WorkerErrors.ValidationWorkerWithPinAlreadyExist;
        }
        
        return Result.Success;
    }

    private static bool ShouldUpdatePin(int currentPin, int newPin)
    {
        return currentPin != newPin;
    }
}