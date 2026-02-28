using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchById;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Commands.DetachBatch;

internal class DetachBatchCommandHandler(
    ISender sender,
    ILogger<DetachBatchCommandHandler> logger,
    IWashingUnitOfWork unitOfWork) : IRequestHandler<DetachBatchCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DetachBatchCommand request, CancellationToken cancellationToken)
    {
        var batch = await sender.Send(new GetBatchByIdQuery(request.BatchId), cancellationToken);
        if (batch.IsError)
        {
            logger.LogError("Washing - Batch with id: {BatchId} was not found in washing.", 
                request.BatchId);
            return batch.Errors;
        }
        
        batch.Value.DetachSisterBatch();
        
        var detachEvent = new SisterBatchDetachedEvent(batch.Value.Id, batch.Value.WashingMachineCode, batch.Value.LineCode);
        unitOfWork.AppendEvent(batch.Value.Id, detachEvent);

        logger.LogInformation("Washing - Batch with Id: {BatchId} detached its sister batch in washing machine with code: {WashingMachineCode}, and line with code: {LineCode}.",
            batch.Value.Id, batch.Value.WashingMachineCode, batch.Value.LineCode);
        
        return Result.Success;
    }
}