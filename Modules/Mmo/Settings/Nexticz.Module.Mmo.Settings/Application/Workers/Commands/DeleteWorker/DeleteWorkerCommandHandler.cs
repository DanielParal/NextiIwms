using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Workers.Queries.GetWorkerById;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Workers.Commands.DeleteWorker;

internal class DeleteWorkerCommandHandler(
    ILogger<DeleteWorkerCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender
    ) : IRequestHandler<DeleteWorkerCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteWorkerCommand request, CancellationToken cancellationToken)
    {
        var worker = await sender.Send(new GetWorkerByIdQuery(request.Id), cancellationToken);

        if (worker.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with ID: {Id}. Nothing to delete.", 
                nameof(Worker), request.Id);
            return worker.Errors;
        }
        
        var workerDeletedEvent = new WorkerDeletedEvent(worker.Value.Id);
        unitOfWork.AppendEvent(worker.Value.Id, workerDeletedEvent);
        return Result.Deleted;
    }
}