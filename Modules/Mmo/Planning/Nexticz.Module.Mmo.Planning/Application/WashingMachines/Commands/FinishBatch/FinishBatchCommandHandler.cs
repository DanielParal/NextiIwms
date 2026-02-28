using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines.Notifications;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachineByLineQueueCode;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.FinishBatch;

internal class FinishBatchCommandHandler(
    ISender sender,
    IClock clock,
    ILogger<FinishBatchCommandHandler> logger,
    IPlanningUnitOfWork planningUnitOfWork,
    IPlanningNotificationCollector notificationCollector) : IRequestHandler<FinishBatchCommand, ErrorOr<(Guid FinishedBatchId, Guid? FinishedSisterBatchId)>>
{
    public async Task<ErrorOr<(Guid FinishedBatchId, Guid? FinishedSisterBatchId)>> Handle(FinishBatchCommand request, CancellationToken cancellationToken)
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
        
        var finishedDate = request.DateActivatedNextBatch ?? clock.UtcNowOffset;
        var finishBatchResult = washingMachine.Value.FinishBatch(batch.Value.Id, finishedDate);
        
        if (finishBatchResult.IsError)
        {
            logger.LogWarning(
                "Cannot finish batch from line queue with code: {Code}. " +
                "Error code: {ErrorCode}, error description: {ErrorDescription}." +
                "BatchId: {BatchId}, SisterBatchId: {SisterBatchId}, KitCode: {KitCode}, PackagingCode: {PackagingCode}, " +
                "KitsCount: {KitsCount}, OptimalKitDuration: {OptimalKitDuration}.", 
                upperLineQueueCode, finishBatchResult.FirstError.Code, finishBatchResult.FirstError.Description,
                batch.Value.Id, batch.Value.SisterBatchId, batch.Value.KitCode, 
                batch.Value.PackagingCode, batch.Value.KitsCount, batch.Value.OptimalKitDuration);
            return finishBatchResult.Errors;
        }

        (Guid FinishedBatchId, Guid? FinishedSisterBatchId) finishedBatches;
        if (batch.Value.HasSisterBatch)
        {
            finishedBatches = FinishSisterBatchesInQueue(batch.Value, upperLineQueueCode, washingMachine.Value, finishedDate);
        }
        else
        {
            finishedBatches = FinishSingleBatchInQueue(
                batch.Value.Id, washingMachine.Value, upperLineQueueCode, finishedDate);
        }

        var isOnlyFinishingBatchAndShouldSendNotification = request.DateActivatedNextBatch is null;
        if (isOnlyFinishingBatchAndShouldSendNotification)
        {
            notificationCollector.AddNotification(
                new BatchWashingFinishedNotification(finishedBatches.FinishedBatchId, finishedBatches.FinishedSisterBatchId, finishedDate));
        }
        
        return finishedBatches;
    }
    
    private (Guid FinishedBatchId, Guid? FinishedSisterBatchId) FinishSingleBatchInQueue(
        Guid batchId, WashingMachine washingMachine, string lineQueueCode, DateTimeOffset finishedDate)
    {
        var singleBatchWashingFinished = new BatchWashingFinishedEvent(batchId, washingMachine.Code, lineQueueCode, finishedDate);
        planningUnitOfWork
            .AppendEvent(washingMachine.Id, singleBatchWashingFinished);
        
        logger.LogInformation("Planning - Batch with id: {BatchId} finished washing on washing machine line with code: {LineCode}.", 
            batchId, lineQueueCode);
        
        return (batchId, null);
    }
    
    private (Guid FinishedBatchId, Guid FinishedSisterBatchId) FinishSisterBatchesInQueue(
        Batch batch, string lineQueueCode, WashingMachine washingMachine, DateTimeOffset finishedDate)
    {
        var sisterLinQueueCode = washingMachine.LineQueues.First(x => x.WashingMachineLineCode != lineQueueCode).WashingMachineLineCode;
        var sisterBatchFinishedEvent = 
            new SisterBatchWashingFinishedEvent(batch.Id, (Guid)batch.SisterBatchId!, washingMachine.Code, lineQueueCode, sisterLinQueueCode, finishedDate);
        
        planningUnitOfWork
            .AppendEvent(washingMachine.Id, sisterBatchFinishedEvent);
        
        logger.LogInformation("Planning - Sister batch with id: {BatchId} with sister batch id: {SisterBatchId} finished washing on washing machine line with code: {LineCode}.", 
            batch.Id, batch.SisterBatchId, sisterLinQueueCode);
        
        return (batch.Id, (Guid)batch.SisterBatchId!);
    }
    
    private async Task<ErrorOr<WashingMachine>> GetWashingMachineByLineQueueCodeAsync(string lineQueueCode, CancellationToken cancellationToken)
    {
        var washingMachine = 
            await sender.Send(
                new GetWashingMachineByLineQueueCodeQuery(lineQueueCode), cancellationToken);

        if (washingMachine.IsError)
        {
            logger.LogWarning("Object {ObjectName} with code: {Code} does not exist. We cannot finish batch.",
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
            logger.LogWarning("Object {ObjectName} with id: {Id} does not exist in the queue: {QueueId}. We cannot remove batch from queue.",
                nameof(Batch), batchId, lineQueueCode);
            return WashingMachineErrors.ValidationBatchIsNotPresentInTheQueue;
        }
        
        return batch;
    }
}