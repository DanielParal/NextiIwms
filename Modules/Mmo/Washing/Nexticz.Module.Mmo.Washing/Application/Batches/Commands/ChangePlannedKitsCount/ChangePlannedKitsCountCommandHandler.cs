using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchById;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Commands.ChangePlannedKitsCount;

internal class ChangePlannedKitsCountCommandHandler(
    ISender sender,
    ILogger<ChangePlannedKitsCountCommandHandler> logger,
    IWashingUnitOfWork unitOfWork) 
    : IRequestHandler<ChangePlannedKitsCountCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ChangePlannedKitsCountCommand request, CancellationToken cancellationToken)
    {
        var batch = await sender.Send(new GetBatchByIdQuery(request.BatchId), cancellationToken);
        if (batch.IsError)
        {
            logger.LogWarning("Washing - {ObjectName} with id {Id} not found. We cannot change planned kits count.",
                nameof(Batch), request.BatchId);
            return BatchErrors.ValidationBatchDoesNotExist;
        }
        
        batch.Value.ChangePlannedKitsCount(request.PlannedKitsCount);
        
        var kitsCountChangedEvent = new BatchPlannedKitsCountChanged(batch.Value.Id, request.PlannedKitsCount);
        unitOfWork.AppendEvent(batch.Value.Id, kitsCountChangedEvent);
        
        logger.LogInformation("Washing - Batch with id {BatchId} planned kits count changed to {PlannedKitsCount}.",
            batch.Value.Id, request.PlannedKitsCount);
        
        return Result.Success;
    }
}