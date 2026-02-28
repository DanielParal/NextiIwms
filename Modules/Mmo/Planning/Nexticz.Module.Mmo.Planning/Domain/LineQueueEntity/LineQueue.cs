using ErrorOr;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;

public class LineQueue : Entity
{
    public string WashingMachineLineCode { get; private set; }
    public bool IsActive { get; private set; }
    
    private readonly List<Batch> _batches = [];
    
    public IReadOnlyList<Batch> Batches => _batches.AsReadOnly();
    
    // We need private constructor due to Marten deserialization
    private LineQueue() {}

    public LineQueue(
        string washingMachineLineCode,
        bool isActive,
        Batch[] batches,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(washingMachineLineCode))
            throw new ArgumentException("WashingMachineLineCode cannot be null or empty.", nameof(washingMachineLineCode));
        
        WashingMachineLineCode = washingMachineLineCode;
        IsActive = isActive;
        _batches = batches.ToList();
    }

    public ErrorOr<Success> CanAddBatch(Batch batch)
    {
        if (batch.Status != BatchStatus.InQueue)
            return LineQueueErrors.ValidationBatchStatusHasToBeInQueue;
        
        return Result.Success;
    }
    
    public ErrorOr<Success> AddBatch(Batch batch)
    {
        var validation = CanAddBatch(batch);
        if (validation.IsError)
            return validation.Errors;
        
        _batches.Add(batch);
        
        return Result.Success;
    }

    public ErrorOr<Success> CanRemoveBatch(Batch? batch)
    {
        if (batch is null)
            return LineQueueErrors.ValidationBatchNotFoundInTheList;

        if (batch.Status == BatchStatus.Washing)
            return LineQueueErrors.ValidationWashingBatchCannotBeRemoved;
        
        return Result.Success;
    }
    
    public ErrorOr<Success> RemoveBatch(Guid batchId)
    {
        var batch = _batches.FirstOrDefault(b => b.Id == batchId);
        var validation = CanRemoveBatch(batch);
        if (validation.IsError)
            return validation.Errors;
        
        _batches.Remove(batch!);
        
        return Result.Success;
    }
    
    public ErrorOr<Success> CanFinishBatch(Batch? batch)
    {
        if (batch is null)
            return LineQueueErrors.ValidationBatchNotFoundInTheList;

        if (batch.Status != BatchStatus.Washing)
            return LineQueueErrors.ValidationBatchCannotBeFinishedIfItIsNotInWashing;
        
        return Result.Success;
    }
    
    public ErrorOr<Success> FinishBatch(Guid batchId, DateTimeOffset finishedDate)
    {
        var batch = _batches.FirstOrDefault(b => b.Id == batchId);
        var validation = CanFinishBatch(batch);
        if (validation.IsError)
            return validation.Errors;
        
        batch!.FinishKit(finishedDate);
        
        return Result.Success;
    }

    public ErrorOr<Success> CanMoveBatch(Batch? batch, int newIndex)
    {
        if (batch == null) 
            return LineQueueErrors.ValidationBatchNotFoundInTheList;
        
        if (newIndex < 0 || newIndex >= _batches.Count) 
            return LineQueueErrors.ValidationBatchCannotBeMovedOutOfRange;
        
        if (_batches[newIndex].Status == BatchStatus.Washing)
            return LineQueueErrors.ValidationBatchCannotBeMovedBeforeBatchWichIsWashing;
        
        return Result.Success;
    }
    
    public ErrorOr<Success> MoveBatch(Guid batchId, int newIndex)
    {
        var batch = _batches.FirstOrDefault(b => b.Id == batchId);
        var validation = CanMoveBatch(batch, newIndex);
        
        if (validation.IsError)
            return validation.Errors;
        
        _batches.Remove(batch!);
        _batches.Insert(newIndex, batch!);
        
        return Result.Success;
    }
    
    public ErrorOr<Success> CanStartBatchWashing(Guid batchId)
    {
        var batchesInQueue = _batches.Where(b => b.Status == BatchStatus.InQueue).ToArray();
        
        if (batchesInQueue.Length == 0 || batchesInQueue[0].Id != batchId)
            return LineQueueErrors.ValidationCannotStartWashingBatchBecauseItIsNotNextInQueue;
        
        return Result.Success;
    }

    public ErrorOr<Success> StartBatchWashing(Guid batchId, DateTimeOffset washingStartedAt)
    {
        var validation = CanStartBatchWashing(batchId);
        if (validation.IsError)
            return validation.Errors;

        FinishPreviousBatchWashing();
        
        var batch = _batches.First(b => b.Id == batchId);
        batch.StartWashing(washingStartedAt);
        return Result.Success;
    }
    
    public void FinishPreviousBatchWashing()
    {
        var washingBatch = GetCurrentWashingBatch();
        if (washingBatch is not null)
            _batches.Remove(washingBatch);
    }

    public Batch? GetCurrentWashingBatch()
    {
        return _batches.FirstOrDefault(x => x.Status == BatchStatus.Washing);
    }

    public void SetStatus(bool isActive)
    {
        IsActive = isActive;
    }
}