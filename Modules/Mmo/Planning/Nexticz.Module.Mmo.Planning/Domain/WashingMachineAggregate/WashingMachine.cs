using ErrorOr;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

public class WashingMachine : AggregateRoot
{
    public string Code { get; private set; }
    public bool IsOneLineMachine { get; private set; }
    public WashingMachineStatus Status { get; private set; }
    private List<LineQueue> _lineQueues = [];
    public IReadOnlyList<LineQueue> LineQueues => _lineQueues.AsReadOnly();

    // We need private constructor due to Marten deserialization
    private WashingMachine() {}
    
    public WashingMachine(
        string code, 
        WashingMachineStatus status,
        LineQueue[] lineQueues,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("MachineCode cannot be null or empty.", nameof(code));
        
        if (lineQueues == null || 
            lineQueues.Length is 0 or > 2)
            throw new ArgumentException("There has to be either one or two washing machine lines.", nameof(lineQueues));
        
        if (lineQueues.Select(x => x.WashingMachineLineCode).Distinct().Count() != lineQueues.Length)
            throw new ArgumentException("WashingMachineLineCodes must contain distinct values.", nameof(lineQueues));

        Code = code;
        Status = status;
        IsOneLineMachine = lineQueues.Length == 1;
        _lineQueues = lineQueues.ToList();
    }

    public ErrorOr<Success> AddBatch(Batch batch, string washingMachineLineCode)
    {
        if (batch.SisterBatchId != null)
            return WashingMachineErrors.ValidationBatchWithSisterIdMustBeScheduledTogether;
        
        var line = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode == washingMachineLineCode);
        if (line is null)
            return WashingMachineErrors.ValidationWashingMachineLineNotFound(washingMachineLineCode);
        
        return line.AddBatch(batch);
    }
    
    public ErrorOr<Success> AddSisterBatches(Batch batch, Batch sisterBatch)
    {
        if (IsOneLineMachine)
            return WashingMachineErrors.ValidationCannotScheduleSimultaneousBatchOnOneLineWashingMachine;
        
        if (batch.Id != sisterBatch.SisterBatchId || 
            batch.SisterBatchId != sisterBatch.Id)
            return WashingMachineErrors.ValidationSisterBatchesIdsMustMatch;
        
        var canAddBatch = _lineQueues[0].CanAddBatch(batch);
        if (canAddBatch.IsError)
            return canAddBatch.Errors;
        
        var canAddSisterBatch = _lineQueues[1].CanAddBatch(sisterBatch);
        if (canAddSisterBatch.IsError)
            return canAddSisterBatch.Errors;
        
        _lineQueues[0].AddBatch(batch);
        _lineQueues[1].AddBatch(sisterBatch);
        
        return Result.Success;
    }

    public ErrorOr<Success> RemoveBatch(Guid batchId)
    {
        var line = _lineQueues.FirstOrDefault(x => x.Batches.Any(b => b.Id == batchId));
        
        if (line is null)
            return WashingMachineErrors.ValidationBatchIdNotFoundInQueue;
        
        var batch = line.Batches.FirstOrDefault(b => b.Id == batchId);
        var canRemoveBatch = line.CanRemoveBatch(batch);
        if (canRemoveBatch.IsError)
            return canRemoveBatch.Errors;
        
        if (batch!.HasSisterBatch)
        {
            var sisterLine = _lineQueues.FirstOrDefault(x => x.Batches.Any(b => b.SisterBatchId == batchId));
            
            if (sisterLine is null)
                return WashingMachineErrors.ValidationBatchIdNotFoundInQueue;
            
            var sisterBatch = sisterLine.Batches.FirstOrDefault(b => b.SisterBatchId == batchId);
            
            var canRemoveSisterBatch = sisterLine.CanRemoveBatch(sisterBatch);
            if (canRemoveSisterBatch.IsError)
                return canRemoveSisterBatch.Errors;
            
            sisterLine.RemoveBatch(sisterBatch!.Id);
        }
        
        line.RemoveBatch(batchId);
        
        return Result.Success;
    }
    
    public ErrorOr<Success> FinishBatch(Guid batchId, DateTimeOffset finishedDate)
    {
        var line = _lineQueues.FirstOrDefault(x => x.Batches.Any(b => b.Id == batchId));
        
        if (line is null)
            return WashingMachineErrors.ValidationBatchIdNotFoundInQueue;
        
        var batch = line.Batches.FirstOrDefault(b => b.Id == batchId);
        var canFinishBatch = line.CanFinishBatch(batch);
        if (canFinishBatch.IsError)
            return canFinishBatch.Errors;
        
        if (batch!.HasSisterBatch)
        {
            var sisterLine = _lineQueues.FirstOrDefault(x => x.Batches.Any(b => b.SisterBatchId == batchId));
            
            if (sisterLine is null)
                return WashingMachineErrors.ValidationBatchIdNotFoundInQueue;
            
            var sisterBatch = sisterLine.Batches.FirstOrDefault(b => b.SisterBatchId == batchId);
            
            var canFinishSisterBatch = sisterLine.CanFinishBatch(sisterBatch);
            if (canFinishSisterBatch.IsError)
                return canFinishSisterBatch.Errors;
            
            sisterLine.FinishBatch(sisterBatch!.Id, finishedDate);
        }
        
        line.FinishBatch(batchId, finishedDate);
        
        return Result.Success;
    }
    
    public ErrorOr<Success> DetachSisterBatch(Guid batchId)
    {
        if (IsOneLineMachine)
            return WashingMachineErrors.ValidationWashingMachineHasOnlyOneLine;
        
        var line = _lineQueues.FirstOrDefault(x => x.Batches.Any(b => b.Id == batchId));
        
        if (line is null)
            return WashingMachineErrors.ValidationBatchIdNotFoundInQueue;
        
        var batch = line.Batches.FirstOrDefault(b => b.Id == batchId);
        if (!batch!.HasSisterBatch)
            return WashingMachineErrors.ValidationBatchIsNotSisterCannotDetach;

        var sisterBatch = _lineQueues
            .SelectMany(x => x.Batches)
            .FirstOrDefault(x => x.Id == batch.SisterBatchId);

        if (sisterBatch is null)
            return WashingMachineErrors.ValidationSisterBatchIdNotFoundInQueue;
        
        batch.SetSisterBatchId(null);
        sisterBatch.SetSisterBatchId(null);
        
        return Result.Success;
    }

    public ErrorOr<Success> UpdateBatchKitsCount(Guid batchId, int countToChange)
    {
        var batch = _lineQueues.SelectMany(x => x.Batches).FirstOrDefault(b => b.Id == batchId);
        
        if (batch is null)
            return WashingMachineErrors.ValidationBatchIdNotFoundInQueue;
        
        var canUpdateKitsCount = batch!.CanUpdateKitsCount(countToChange);
        if (canUpdateKitsCount.IsError)
            return canUpdateKitsCount.Errors;
        
        if (batch.HasSisterBatch)
        {
            var sisterBatch = _lineQueues.SelectMany(x => x.Batches).FirstOrDefault(b => b.Id == batch.SisterBatchId);
            
            if (sisterBatch is null)
                return WashingMachineErrors.ValidationSisterBatchIdNotFoundInQueue;
            
            var canDecreaseSisterKitsCount = sisterBatch!.CanUpdateKitsCount(countToChange);
            if (canDecreaseSisterKitsCount.IsError)
                return canDecreaseSisterKitsCount.Errors;
            
            sisterBatch.UpdateKitsCount(countToChange);
        }
            
        batch.UpdateKitsCount(countToChange);
        
        return Result.Success;
    }
    
    public ErrorOr<Success> MoveBatch(Guid batchId, int newIndex)
    {
        var line = _lineQueues.FirstOrDefault(x => x.Batches.Any(b => b.Id == batchId));
        
        if (line is null)
            return WashingMachineErrors.ValidationBatchIdNotFoundInQueue;
        
        var batch = line.Batches.First(b => b.Id == batchId);
        
        var canMoveBatch = line.CanMoveBatch(batch, newIndex);
        if (canMoveBatch.IsError)
            return canMoveBatch.Errors;
        
        var result = line.MoveBatch(batchId, newIndex);
        if (result.IsError)
            return result.Errors;
        
        return Result.Success;
    }

    public ErrorOr<Success> StartBatchWashing(Guid batchId, DateTimeOffset washingStartedAt)
    {
        var line = _lineQueues.FirstOrDefault(x => x.Batches.Any(b => b.Id == batchId));
        
        if (line is null)
            return WashingMachineErrors.ValidationBatchIdNotFoundInQueue;

        var currentBatchInWashing = line.GetCurrentWashingBatch();

        if (currentBatchInWashing is not null && currentBatchInWashing.HasSisterBatch)
        {
            var sisterLine = _lineQueues.First(x => !x.WashingMachineLineCode.Equals(line.WashingMachineLineCode, StringComparison.InvariantCultureIgnoreCase));
            sisterLine.FinishPreviousBatchWashing();
        }
        
        var result = line.StartBatchWashing(batchId, washingStartedAt);
        
        return result.IsError ? result.Errors : Result.Success;
    }
    
    public ErrorOr<Success> StartSisterBatchWashing(Guid batchId, Guid sisterBatchId, DateTimeOffset washingStartedAt)
    {
        var line = _lineQueues.FirstOrDefault(x => x.Batches.Any(b => b.Id == batchId));
        
        if (line is null)
            return WashingMachineErrors.ValidationBatchIdNotFoundInQueue;
        
        var sisterLine = _lineQueues.FirstOrDefault(x => x.Batches.Any(b => b.Id == sisterBatchId));
        
        if (sisterLine is null)
            return WashingMachineErrors.ValidationBatchIdNotFoundInQueue;
        
        var canStartWashingBatch = line.CanStartBatchWashing(batchId);
        if (canStartWashingBatch.IsError)
            return canStartWashingBatch.Errors;
        
        var canStartWashingSisterBatch = sisterLine.CanStartBatchWashing(sisterBatchId);
        if (canStartWashingSisterBatch.IsError)
            return canStartWashingSisterBatch.Errors;
        
        line.StartBatchWashing(batchId, washingStartedAt);
        sisterLine.StartBatchWashing(sisterBatchId, washingStartedAt);
        
        return Result.Success;
    }
    
    public void Apply(WashingMachineCreatedEvent @event)
    {
        Id = @event.Id;
        Code = @event.Code;
        Status = @event.Status;
        IsOneLineMachine = @event.IsOneLineMachine;
        _lineQueues = @event.LineQueues.Select(x => new LineQueue(x.WashingMachineLineCode, x.IsActive, x.Batches.ToArray())).ToList();
    }
    
    public void Apply(WashingMachineStatusUpdatedEvent @event)
    {
        Code = @event.Code;
        Status = @event.Status;
        foreach (var lineQueueFromEvent in @event.LineQueues)
        {
            var lineQueue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(lineQueueFromEvent.WashingMachineLineCode, StringComparison.InvariantCultureIgnoreCase));
            lineQueue?.SetStatus(lineQueueFromEvent.IsActive);
        }
    }
    
    public void Apply(BatchCreatedEvent @event)
    {
        var batch = new Batch(
            null, 
            @event.DepositorCode,
            @event.KitCode, 
            @event.KitNumber,
            @event.KitsCount, 
            @event.PackagingCode,
            @event.PackagingHeight,
            @event.DefiningPackagingCode,
            @event.KitSapDefinitionCode,
            @event.OptimalKitDuration,
            id: @event.BatchId);

        var lineQueue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode == @event.LineQueueCode);

        lineQueue?.AddBatch(batch);
    }
    
    public void Apply(SisterBatchCreatedEvent @event)
    {
        var batch = new Batch(
            @event.SisterBatchId, 
            @event.DepositorCode,
            @event.KitCode, 
            @event.KitNumber,
            @event.KitsCount, 
            @event.PackagingCode, 
            @event.PackagingHeight,
            @event.DefiningPackagingCode,
            @event.KitSapDefinitionCode,
            @event.OptimalKitDuration,
            id: @event.BatchId);

        var lineQueue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode == @event.LineQueueCode);

        lineQueue?.AddBatch(batch);
        
        var sisterBatch = new Batch(
            @event.BatchId, 
            @event.DepositorCode,
            @event.KitCode, 
            @event.KitNumber,
            @event.KitsCount, 
            @event.SisterPackagingCode, 
            @event.SisterPackagingHeight,
            @event.DefiningPackagingCode,
            @event.KitSapDefinitionCode,
            @event.OptimalSisterKitDuration,
            id: @event.SisterBatchId);

        var sisterLineQueue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode == @event.SisterLineQueueCode);

        sisterLineQueue?.AddBatch(sisterBatch);
    }
    
    public void Apply(BatchRemovedEvent @event)
    {
        var queue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.LineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        if (queue is null)
            return;
        
        var batch = queue.Batches.FirstOrDefault(x => x.Id == @event.BatchId);
        if (batch is null)
            return;
        
        queue.RemoveBatch(@event.BatchId);
    }
    
    public void Apply(SisterBatchRemovedEvent @event)
    {
        var queue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.LineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        var batch = queue?.Batches.FirstOrDefault(x => x.Id == @event.BatchId);
        if (batch is null)
            return;
        
        queue?.RemoveBatch(@event.BatchId);
        
        var sisterQueue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.SisterLineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        var sisterBatch = sisterQueue?.Batches.FirstOrDefault(x => x.Id == @event.SisterBatchId);
        if (sisterBatch is null)
            return;
        
        sisterQueue?.RemoveBatch(@event.SisterBatchId);
    }
    
    public void Apply(BatchKitsCountChangedEvent @event)
    {
        var queue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.LineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        var batch = queue?.Batches.FirstOrDefault(x => x.Id == @event.BatchId);
        batch?.UpdateKitsCount(@event.CountToChange);
    }
    
    public void Apply(SisterBatchKitsCountChangedEvent @event)
    {
        var queue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.LineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        var batch = queue?.Batches.FirstOrDefault(x => x.Id == @event.BatchId);
        batch?.UpdateKitsCount(@event.CountToChange);
        
        var sisterQueue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.SisterLineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        var sisterBatch = sisterQueue?.Batches.FirstOrDefault(x => x.Id == @event.SisterBatchId);
        sisterBatch?.UpdateKitsCount(@event.CountToChange);
    }
    
    public void Apply(BatchInQueueMovedEvent @event)
    {
        var queue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.LineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        var batch = queue?.Batches.FirstOrDefault(x => x.Id == @event.BatchId);
        if (batch is null)
            return;
        
        queue?.MoveBatch(@event.BatchId, @event.NewIndex);
    }
    
    public void Apply(SisterBatchInQueueMovedEvent @event)
    {
        var queue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.LineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        var batch = queue?.Batches.FirstOrDefault(x => x.Id == @event.BatchId);
        if (batch is null)
            return;
        
        queue?.MoveBatch(@event.BatchId, @event.NewIndex);
        
        var sisterQueue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.SisterLineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        var sisterBatch = sisterQueue?.Batches.FirstOrDefault(x => x.Id == @event.SisterBatchId);
        if (sisterBatch is null)
            return;
        
        sisterQueue?.MoveBatch(@event.SisterBatchId, @event.SisterNewIndex);
    }
    
    public void Apply(SisterBatchDetachedEvent @event)
    {
        var queue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.LineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        if (queue is null)
            return;
        
        var batch = queue.Batches.FirstOrDefault(x => x.Id == @event.BatchId);
        if (batch is null)
            return;
        
        var sisterQueue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.SisterLineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        if (sisterQueue is null)
            return;
        
        var sisterBatch = sisterQueue.Batches.FirstOrDefault(x => x.Id == @event.SisterBatchId);
        if (sisterBatch is null)
            return;
        
        batch.SetSisterBatchId(null);
        sisterBatch.SetSisterBatchId(null);
    }
    
    public void Apply(BatchWashingStartedEvent @event)
    {
        var queue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.LineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        var batch = queue?.Batches.FirstOrDefault(x => x.Id == @event.BatchId);
        batch?.StartWashing(@event.WashingStartedAt);
    }
    
    public void Apply(SisterBatchWashingStartedEvent @event)
    {
        var queue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.LineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        var batch = queue?.Batches.FirstOrDefault(x => x.Id == @event.BatchId);
        batch?.StartWashing(@event.WashingStartedAt);
        
        var sisterQueue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.SisterLineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        var sisterBatch = sisterQueue?.Batches.FirstOrDefault(x => x.Id == @event.SisterBatchId);
        sisterBatch?.StartWashing(@event.WashingStartedAt);
    }
    
    public void Apply(BatchWashingFinishedEvent @event)
    {
        var queue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.LineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        if (queue is null)
            return;
        
        var batch = queue.Batches.FirstOrDefault(x => x.Id == @event.BatchId);
        if (batch is null)
            return;
        
        queue.FinishPreviousBatchWashing();
    }
    
    public void Apply(SisterBatchWashingFinishedEvent @event)
    {
        var queue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.LineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        var batch = queue?.Batches.FirstOrDefault(x => x.Id == @event.BatchId);
        if (batch is null)
            return;
        
        queue?.FinishPreviousBatchWashing();
        
        var sisterQueue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.SisterLineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        var sisterBatch = sisterQueue?.Batches.FirstOrDefault(x => x.Id == @event.SisterBatchId);
        if (sisterBatch is null)
            return;
        
        sisterQueue?.FinishPreviousBatchWashing();
    }
    
    public void Apply(BatchKitFinishedEvent @event)
    {
        var queue = _lineQueues.FirstOrDefault(x => x.WashingMachineLineCode.Equals(@event.LineQueueCode, StringComparison.InvariantCultureIgnoreCase));
        if (queue is null)
            return;
        
        var batch = queue.Batches.FirstOrDefault(x => x.Id == @event.BatchId);
        if (batch is null)
            return;

        batch.FinishKit(@event.DateFinished);
    }
}