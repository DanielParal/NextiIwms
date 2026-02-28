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

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.ChangeBatchKitsCount;

internal class ChangeBatchKitsCountCommandHandler(
    ISender sender,
    ILogger<ChangeBatchKitsCountCommandHandler> logger,
    IPlanningUnitOfWork planningUnitOfWork,
    IPlanningNotificationCollector notificationCollector
    ) : IRequestHandler<ChangeBatchKitsCountCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(ChangeBatchKitsCountCommand request, CancellationToken cancellationToken)
    {
        var upperLineQueueCode = request.LineQueueCode.ToUpperInvariant();
        // 1. Get washing machine by line queue
        var washingMachine = await GetWashingMachineByLineQueueCodeAsync(upperLineQueueCode, cancellationToken);
        if(washingMachine.IsError)
            return washingMachine.Errors;
        
        // 2. Get batch by id
        var batch = GetBatchFromQueue(washingMachine.Value, request.BatchId, upperLineQueueCode);
        if (batch.IsError)
            return batch.Errors;
        
        // 3. Update batch kits count and create event
        var removeBatchResult = washingMachine.Value.UpdateBatchKitsCount(batch.Value.Id, request.CountToChange);
        
        if (removeBatchResult.IsError)
        {
            logger.LogWarning(
                "Cannot change batch count from line queue with code: {Code}. " +
                "Error code: {ErrorCode}, error description: {ErrorDescription}." +
                "KitCode: {KitCode}, PackagingCode: {PackagingCode}, CurrentKitsCount: {KitsCount}, " +
                "RequestedKitsCountChange: {KitCountToChange}, OptimalKitDuration: {OptimalKitDuration}.", 
                upperLineQueueCode, removeBatchResult.FirstError.Code, removeBatchResult.FirstError.Description,
                batch.Value.KitCode, batch.Value.PackagingCode, batch.Value.KitsCount, 
                request.CountToChange, batch.Value.OptimalKitDuration);
            return removeBatchResult.Errors;
        }
        
        if (!batch.Value.HasSisterBatch)
        {
            // 4.1. update batch kits count 
            UpdateSingleBatchKitsCount(
                batch.Value, washingMachine.Value.Code, upperLineQueueCode, request.CountToChange, 
                washingMachine.Value.Id);
            return Result.Updated;
        }
        
        // 4.2. update sister batch kits count
        UpdateSisterBatchKitsCount(batch.Value, upperLineQueueCode, request.CountToChange, 
            washingMachine.Value);
        return Result.Updated;
    }

    private void UpdateSisterBatchKitsCount(
        Batch batch, string lineQueueCode, int countToChange, WashingMachine washingMachine)
    {
        var sisterLinQueueCode = washingMachine.LineQueues.First(x => x.WashingMachineLineCode != lineQueueCode).WashingMachineLineCode;
        var sisterBatchKitsCountUpdatedEvent = 
            new SisterBatchKitsCountChangedEvent(batch.Id,  batch.SisterBatchId!.Value, washingMachine.Code, lineQueueCode, sisterLinQueueCode, countToChange);
        
        planningUnitOfWork
            .AppendEvent(washingMachine.Id, sisterBatchKitsCountUpdatedEvent);
        
        if (batch.Status == BatchStatus.Washing)
            notificationCollector.AddNotification(new BatchKitsCountChangedNotification(batch.Id, batch.SisterBatchId!.Value, countToChange));
    }

    private void UpdateSingleBatchKitsCount(
        Batch batch, string washingMachineCode, string lineQueueCode, int countToChange, Guid washingMachineId)
    {
        var singleBatchUpdateEvent = new BatchKitsCountChangedEvent(batch.Id, washingMachineCode, lineQueueCode, countToChange);
        planningUnitOfWork
            .AppendEvent(washingMachineId, singleBatchUpdateEvent);
        
        if (batch.Status == BatchStatus.Washing)
            notificationCollector.AddNotification(new BatchKitsCountChangedNotification(batch.Id, null, countToChange));
    }

    private async Task<ErrorOr<WashingMachine>> GetWashingMachineByLineQueueCodeAsync(string lineQueueCode, CancellationToken cancellationToken)
    {
        var washingMachine = 
            await sender.Send(
                new GetWashingMachineByLineQueueCodeQuery(lineQueueCode), cancellationToken);

        if (washingMachine.IsError)
        {
            logger.LogWarning("Object {ObjectName} with code: {Code} does not exist. We cannot remove batch from queue.",
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