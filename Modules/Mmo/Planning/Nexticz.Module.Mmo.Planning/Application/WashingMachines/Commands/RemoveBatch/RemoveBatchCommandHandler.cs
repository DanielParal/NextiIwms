using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachineByLineQueueCode;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.RemoveBatch;

internal class RemoveBatchCommandHandler(
    ISender sender,
    ILogger<RemoveBatchCommandHandler> logger,
    IPlanningUnitOfWork planningUnitOfWork
    )
    : IRequestHandler<RemoveBatchCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(RemoveBatchCommand request, CancellationToken cancellationToken)
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
        
        // 3. remove batch from queue
        var removeBatchResult = washingMachine.Value.RemoveBatch(batch.Value.Id);
        
        if (removeBatchResult.IsError)
        {
            logger.LogWarning(
                "Cannot remove batch from line queue with code: {Code}. " +
                "Error code: {ErrorCode}, error description: {ErrorDescription}." +
                "KitCode: {KitCode}, PackagingCode: {PackagingCode}, KitsCount: {KitsCount}, OptimalKitDuration: {OptimalKitDuration}.", 
                upperLineQueueCode, removeBatchResult.FirstError.Code, removeBatchResult.FirstError.Description,
                batch.Value.KitCode, batch.Value.PackagingCode, batch.Value.KitsCount, batch.Value.OptimalKitDuration);
            return removeBatchResult.Errors;
        }

        if (!batch.Value.HasSisterBatch)
        {
            RemoveSingleBatchFromQueue(batch.Value.Id, washingMachine.Value.Code, upperLineQueueCode, washingMachine.Value.Id);
            return Result.Deleted;
        }

        RemoveSisterBatchesFromQueue(batch.Value, upperLineQueueCode, washingMachine.Value);
        return Result.Deleted;
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

    private void RemoveSingleBatchFromQueue(
        Guid batchId, string washingMachineCode, string lineQueueCode, Guid washingMachineId)
    {
        var singleBatchRemovedEvent = new BatchRemovedEvent(batchId, washingMachineCode, lineQueueCode);
        planningUnitOfWork
            .AppendEvent(washingMachineId, singleBatchRemovedEvent);
    }
    
    private void RemoveSisterBatchesFromQueue(
        Batch batch, string lineQueueCode, WashingMachine washingMachine)
    {
        var sisterLinQueueCode = washingMachine.LineQueues.First(x => x.WashingMachineLineCode != lineQueueCode).WashingMachineLineCode;
        var sisterBatchRemovedEvent = new SisterBatchRemovedEvent(batch.Id, (Guid)batch.SisterBatchId!, washingMachine.Code, lineQueueCode, sisterLinQueueCode);
        
        planningUnitOfWork
            .AppendEvent(washingMachine.Id, sisterBatchRemovedEvent);
    }
}