using ErrorOr;
using JasperFx.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachineByLineQueueCode;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.MoveBatchInQueue;

internal class MoveBatchInQueueCommandHandler(
    ISender sender, 
    ILogger<MoveBatchInQueueCommandHandler> logger,
    IPlanningUnitOfWork planningUnitOfWork
    )
    : IRequestHandler<MoveBatchInQueueCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(MoveBatchInQueueCommand request, CancellationToken cancellationToken)
    {
        var upperLineQueueCode = request.LineQueueCode.ToUpperInvariant();
        // 1. Get washing machine by line queue
        var washingMachine = await GetWashingMachineByLineQueueCodeAsync(upperLineQueueCode, cancellationToken);
        if(washingMachine.IsError)
            return washingMachine.Errors;
        
        // 2. Get batch by id
        var batchWithQueue = GetBatchWithLineQueueFromQueue(washingMachine.Value, request.BatchId, upperLineQueueCode);
        if (batchWithQueue.IsError)
            return batchWithQueue.Errors;
        
        // 3. Move batch in queue create event
        if (!batchWithQueue.Value.Batch.HasSisterBatch)
        {
            return MoveSingleBatch(washingMachine.Value, batchWithQueue.Value.Batch, batchWithQueue.Value.LineQueue, request.Movement);
        }

        return MoveSisterBatch(washingMachine.Value, batchWithQueue.Value.Batch, batchWithQueue.Value.LineQueue, request.Movement);
    }
    
    private ErrorOr<Success> MoveSisterBatch(WashingMachine washingMachine, Domain.BatchEntity.Batch batch, Domain.LineQueueEntity.LineQueue lineQueue, InQueueMovementContract movement)
    {
        if (washingMachine.IsOneLineMachine)
        {
            logger.LogWarning("Planning - Cannot move sister batch with id {BatchId} in washing machine with code: {WashingMachine}. " +
                              "Washing machine has only one line.", 
                batch.Id, washingMachine.Code);
            return WashingMachineErrors.ValidationSingleLineWashingMachine;
        }
        
        var sisterLineQueue = washingMachine.LineQueues.First(x => x.WashingMachineLineCode != lineQueue.WashingMachineLineCode);
        var sisterBatch = sisterLineQueue.Batches.First(x => x.Id == batch.SisterBatchId);
        
        var index = GetSisterNewIndex(batch, lineQueue, sisterBatch, sisterLineQueue, movement);
        if (index.IsError)
            return index.Errors;
        
        var sisterIndex = GetSisterNewIndex(sisterBatch, sisterLineQueue, batch, lineQueue, movement);
        if (sisterIndex.IsError)
            return sisterIndex.Errors;

        var canMoveBatchInQueue = lineQueue.CanMoveBatch(batch, index.Value);
        if (canMoveBatchInQueue.IsError)
        {
            logger.LogWarning(
                "Cannot move sister batch in line queue with code: {Code}. " +
                "Error code: {ErrorCode}, error description: {ErrorDescription}." +
                "BatchId: {BatchId}, SisterBatchId: {SisterBatchId}, RequestedIndex: {RequestedIndex}.", 
                lineQueue.WashingMachineLineCode, canMoveBatchInQueue.FirstError.Code, canMoveBatchInQueue.FirstError.Description,
                batch.Id, batch.SisterBatchId, index.Value);
            return canMoveBatchInQueue.Errors;
        }
        
        var canMoveSisterBatchInQueue = sisterLineQueue.CanMoveBatch(sisterBatch, sisterIndex.Value);
        if (canMoveSisterBatchInQueue.IsError)
        {
            logger.LogWarning(
                "Cannot move sister batch in line queue with code: {Code}. " +
                "Error code: {ErrorCode}, error description: {ErrorDescription}." +
                "BatchId: {BatchId}, SisterBatchId: {SisterBatchId} RequestedIndex: {RequestedIndex}.", 
                lineQueue.WashingMachineLineCode, canMoveBatchInQueue.FirstError.Code, canMoveBatchInQueue.FirstError.Description,
                batch.Id, batch.SisterBatchId, index.Value);
            return canMoveSisterBatchInQueue.Errors;
        }
        
        var sisterBatchInQueueMoved = new SisterBatchInQueueMovedEvent(
            washingMachine.Code,
            batch.Id, lineQueue.WashingMachineLineCode, index.Value,
            sisterBatch.Id, sisterLineQueue.WashingMachineLineCode, sisterIndex.Value);
        planningUnitOfWork
            .AppendEvent(washingMachine.Id, sisterBatchInQueueMoved);
        
        return Result.Success;
    }

    private ErrorOr<Success> MoveSingleBatch(WashingMachine washingMachine, Domain.BatchEntity.Batch batch, Domain.LineQueueEntity.LineQueue lineQueue, InQueueMovementContract movement)
    {
        var index = GetNewIndex(batch, lineQueue, movement);
        if (index.IsError)
            return index.Errors;
        
        var moveBatchInQueue = washingMachine.MoveBatch(batch.Id, index.Value);
        
        if (moveBatchInQueue.IsError)
        {
            logger.LogWarning(
                "Cannot move batch in line queue with code: {Code}. " +
                "Error code: {ErrorCode}, error description: {ErrorDescription}." +
                "KitCode: {KitCode}, PackagingCode: {PackagingCode}, CurrentKitsCount: {KitsCount}, " +
                "RequestedIndex: {RequestedIndex}, OptimalKitDuration: {OptimalKitDuration}.", 
                lineQueue.WashingMachineLineCode, moveBatchInQueue.FirstError.Code, moveBatchInQueue.FirstError.Description,
                batch.KitCode, batch.PackagingCode, batch.KitsCount, 
                index, batch.OptimalKitDuration);
            return moveBatchInQueue.Errors;
        }
        
        var batchInQueueMovedEvent = new BatchInQueueMovedEvent(batch.Id, washingMachine.Code, lineQueue.WashingMachineLineCode, index.Value);
        planningUnitOfWork
            .AppendEvent(washingMachine.Id, batchInQueueMovedEvent);
        
        return Result.Success;
    }
    
    private async Task<ErrorOr<WashingMachine>> GetWashingMachineByLineQueueCodeAsync(string lineQueueCode, CancellationToken cancellationToken)
    {
        var washingMachine = 
            await sender.Send(
                new GetWashingMachineByLineQueueCodeQuery(lineQueueCode), cancellationToken);

        if (washingMachine.IsError)
        {
            logger.LogWarning("Object {ObjectName} with code: {Code} does not exist. We cannot move batch in queue.",
                nameof(Domain.LineQueueEntity.LineQueue), lineQueueCode);
            return WashingMachineErrors.ValidationLineQueueDoesNotExist;
        }
        
        return washingMachine.Value;
    }
    
    private ErrorOr<(Domain.BatchEntity.Batch Batch, Domain.LineQueueEntity.LineQueue LineQueue)> GetBatchWithLineQueueFromQueue(WashingMachine washingMachine, Guid batchId, string lineQueueCode)
    {
        var lineQueue = washingMachine
            .LineQueues
            .FirstOrDefault(x => x.WashingMachineLineCode == lineQueueCode);
        
        if (lineQueue is null)
        {
            logger.LogWarning("Object {ObjectName} with code: {Code} does not exist. We cannot move batch in queue.",
                nameof(LineQueue), lineQueueCode);
            return WashingMachineErrors.ValidationLineQueueDoesNotExist;
        }
        
        var batch = lineQueue
            .Batches
            .FirstOrDefault(x => x.Id == batchId);

        if (batch is null)
        {
            logger.LogWarning("Object {ObjectName} with id: {Id} does not exist in the queue: {QueueCode}. We cannot move batch in queue.",
                nameof(Batch), batchId, lineQueueCode);
            return WashingMachineErrors.ValidationBatchIsNotPresentInTheQueue;
        }
        
        return (batch, lineQueue);
    }
    
    private ErrorOr<int> GetNewIndex(Batch batch, LineQueue lineQueue, InQueueMovementContract movement)
    {
        if (batch.Status == BatchStatus.Washing)
        {
            logger.LogWarning(
                "Cannot move batch batch which is in washing. BatchId: {BatchId}, LineQueueCode: {LineQueueCode}.",
                batch.Id, lineQueue.WashingMachineLineCode);
            return WashingMachineErrors.ValidationCannotMoveSingleBatch;
        }

        var currentIndex = lineQueue.Batches.ToArray().IndexOf(batch);
        var newIndex = movement switch
        {
            InQueueMovementContract.FirstInQueue => 0,
            InQueueMovementContract.OneUp => currentIndex - 1,
            InQueueMovementContract.OneDown => currentIndex + 1,
            _ => throw new ArgumentOutOfRangeException(nameof(movement), movement, null)
        };

        var isFirstBatchWashing = lineQueue.Batches[0].Status == BatchStatus.Washing;
        var minIndex = isFirstBatchWashing ? 1 : 0;
        var maxIndex = lineQueue.Batches.Count - 1;

        return Math.Clamp(newIndex, minIndex, maxIndex);
    }
    
    private ErrorOr<int> GetSisterNewIndex(Batch batch, LineQueue lineQueue, Batch sisterBatch, LineQueue sisterLineQueue, InQueueMovementContract movement)
    {
        if (batch.Status == BatchStatus.Washing)
        {
            logger.LogWarning(
                "Cannot move batch batch which is in washing. BatchId: {BatchId}, LineQueueCode: {LineQueueCode}.",
                batch.Id, lineQueue.WashingMachineLineCode);
            return WashingMachineErrors.ValidationCannotMoveSingleBatch;
        }

        var currentIndex = lineQueue.Batches.ToArray().IndexOf(batch);
        var sisterCurrentIndex = sisterLineQueue.Batches.ToArray().IndexOf(sisterBatch);
        
        var newIndex = movement switch
        {
            InQueueMovementContract.FirstInQueue => 0,
            InQueueMovementContract.OneUp => GetOneUpIndex(lineQueue, sisterLineQueue, currentIndex, sisterCurrentIndex),
            InQueueMovementContract.OneDown => GetOneDownIndex(lineQueue, sisterLineQueue, currentIndex, sisterCurrentIndex),
            _ => throw new ArgumentOutOfRangeException(nameof(movement), movement, null)
        };

        var isFirstBatchWashing = lineQueue.Batches[0].Status == BatchStatus.Washing;
        var minIndex = isFirstBatchWashing ? 1 : 0;
        var maxIndex = lineQueue.Batches.Count - 1;

        return Math.Clamp(newIndex, minIndex, maxIndex);
    }

    private static int GetOneUpIndex(LineQueue lineQueue, LineQueue sisterLineQueue, int indexOfCurrentBatch, int indexOfCurrentSisterBatch)
    {
        var previousBatch = GetPreviousBatch(lineQueue, indexOfCurrentBatch);
        var previousSisterBatch = GetPreviousBatch(sisterLineQueue, indexOfCurrentSisterBatch);
        
        if (previousBatch is null)
            return 0;
        
        if (previousSisterBatch is null)
            return indexOfCurrentBatch - 1;

        if (previousBatch.HasSisterBatch && !previousSisterBatch.HasSisterBatch)
            return indexOfCurrentBatch;

        return indexOfCurrentBatch - 1;
    }
    
    private static int GetOneDownIndex(LineQueue lineQueue, LineQueue sisterLineQueue, int indexOfCurrentBatch, int indexOfCurrentSisterBatch)
    {
        var nextBatch = GetNextBatch(lineQueue, indexOfCurrentBatch);
        var nextSisterBatch = GetNextBatch(sisterLineQueue, indexOfCurrentSisterBatch);
        
        if (nextBatch is null)
            return indexOfCurrentBatch;
        
        if (nextSisterBatch is null)
            return indexOfCurrentBatch + 1;

        if (nextBatch.HasSisterBatch && !nextSisterBatch.HasSisterBatch)
            return indexOfCurrentBatch;

        return indexOfCurrentBatch + 1;
    }
    
    private static Batch? GetPreviousBatch(LineQueue lineQueue, int indexOfCurrentBatch)
    {
        if (indexOfCurrentBatch == 0)
            return null;

        return lineQueue.Batches[indexOfCurrentBatch - 1];
    }

    private static Batch? GetNextBatch(LineQueue lineQueue, int indexOfCurrentBatch)
    {
        if (indexOfCurrentBatch >= lineQueue.Batches.Count - 1)
            return null;

        return lineQueue.Batches[indexOfCurrentBatch + 1];
    }
}