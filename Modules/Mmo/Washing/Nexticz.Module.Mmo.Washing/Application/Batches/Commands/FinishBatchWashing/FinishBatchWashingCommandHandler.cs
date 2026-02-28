using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Washing.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Washing.Contracts.Batches.Notifications;
using Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchById;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Commands.FinishBatchWashing;

internal class FinishBatchWashingCommandHandler(
    ISender sender,
    ILogger<FinishBatchWashingCommandHandler> logger,
    IWashingUnitOfWork unitOfWork)
    : IRequestHandler<FinishBatchWashingCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(FinishBatchWashingCommand request, CancellationToken cancellationToken)
    {
        var batch = await sender.Send(new GetBatchByIdQuery(request.BatchId), cancellationToken);
        if (batch.IsError)
        {
            logger.LogError("Batch with id: {BatchId} was not found in washing.", 
                request.BatchId);
            return batch.Errors;
        }

        var batchDeletedEvent = new BatchFinishedEvent(batch.Value.Id, request.DateFinished);
        unitOfWork.AppendEvent(batch.Value.Id, batchDeletedEvent);
        
        logger.LogInformation("Washing - Batch with id {BatchId} finished in washing at DateTimeOffset: {DateFinished}.",
            batch.Value.Id, request.DateFinished);
        
        return Result.Success;
    }
}