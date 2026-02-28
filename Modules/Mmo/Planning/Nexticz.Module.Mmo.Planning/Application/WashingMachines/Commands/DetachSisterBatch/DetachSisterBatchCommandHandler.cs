using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines.Notifications;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachineByLineQueueCode;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.DetachSisterBatch;

internal class DetachSisterBatchCommandHandler(
    ISender sender,
    ILogger<DetachSisterBatchCommandHandler> logger,
    IPlanningUnitOfWork planningUnitOfWork,
    IPlanningNotificationCollector notificationCollector) 
    : IRequestHandler<DetachSisterBatchCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DetachSisterBatchCommand request, CancellationToken cancellationToken)
    {
        var upperLineQueueCode = request.LineQueueCode.ToUpperInvariant();
        // 1. get washing machine by line queue code
        var washingMachine = await GetWashingMachineByLineQueueCodeAsync(upperLineQueueCode, cancellationToken);
        if(washingMachine.IsError)
            return washingMachine.Errors;
        
        // 2. verify batch id is present in queue
        var batch = GetBatchFromQueue(washingMachine.Value, request.BatchId, upperLineQueueCode);
        if (batch.IsError)
            return batch.Errors;

        if (!batch.Value.HasSisterBatch)
        {
            logger.LogWarning("Cannot detach batch with id: {BatchId} in line with code: {LineCode} because batch is not sister batch.",
                batch.Value.Id, upperLineQueueCode);
            return WashingMachineErrors.ValidationBatchIsNotSisterBatch;
        }

        var sisterBatchId = batch.Value.SisterBatchId;
        var detachBatchResult = washingMachine.Value.DetachSisterBatch(batch.Value.Id);
        
        if (detachBatchResult.IsError)
        {
            logger.LogWarning(
                "Cannot detach sister batch from line queue with code: {Code}. " +
                "Error code: {ErrorCode}, error description: {ErrorDescription}." +
                "BatchId: {BatchId}, SisterBatchId: {SisterBatchId}.", 
                upperLineQueueCode, detachBatchResult.FirstError.Code, detachBatchResult.FirstError.Description,
                batch.Value.Id, batch.Value.SisterBatchId);
            return detachBatchResult.Errors;
        }
        
        var sisterLinQueueCode = washingMachine.Value.LineQueues.First(x => x.WashingMachineLineCode != upperLineQueueCode).WashingMachineLineCode;
        var detachSisterBatchEvent = new SisterBatchDetachedEvent(
            batch.Value.Id, (Guid)sisterBatchId!, washingMachine.Value.Code,
            upperLineQueueCode, sisterLinQueueCode);
        planningUnitOfWork
            .AppendEvent(washingMachine.Value.Id, detachSisterBatchEvent);

        if (batch.Value.Status == BatchStatus.Washing)
        {
            notificationCollector.AddNotification(
                new SisterBatchDetachedNotification(batch.Value.Id, (Guid)sisterBatchId!, washingMachine.Value.Code,
                    upperLineQueueCode, sisterLinQueueCode));
        }
            
        logger.LogInformation("Planning - Batch with id: {BatchId} and sister batch id: {SisterBatchId} detached on washing machine with code: {WashingMachineCode}.", 
            batch.Value.Id, (Guid)sisterBatchId!, washingMachine.Value.Code);
        
        return Result.Success;
    }
    
    private async Task<ErrorOr<WashingMachine>> GetWashingMachineByLineQueueCodeAsync(string lineQueueCode, CancellationToken cancellationToken)
    {
        var washingMachine = 
            await sender.Send(
                new GetWashingMachineByLineQueueCodeQuery(lineQueueCode), cancellationToken);

        if (washingMachine.IsError)
        {
            logger.LogWarning("Object {ObjectName} with code: {Code} does not exist. We cannot detach sister batch.",
                nameof(LineQueue), lineQueueCode);
            return WashingMachineErrors.ValidationLineQueueDoesNotExist;
        }
        
        return washingMachine.Value;
    }
    
    private ErrorOr<Batch> GetBatchFromQueue(WashingMachine washingMachine, Guid batchId, string lineQueueCode)
    {
        var batch = washingMachine
            .LineQueues
            .First(x => x.WashingMachineLineCode == lineQueueCode)
            .Batches
            .FirstOrDefault(x => x.Id == batchId);

        if (batch is null)
        {
            logger.LogWarning("Object {ObjectName} with id: {Id} does not exist in the queue: {QueueId}. We cannot detach sister batch.",
                nameof(Batch), batchId, lineQueueCode);
            return WashingMachineErrors.ValidationBatchIsNotPresentInTheQueue;
        }
        
        return batch;
    }
}