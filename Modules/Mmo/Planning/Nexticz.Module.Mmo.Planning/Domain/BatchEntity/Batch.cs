using ErrorOr;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Planning.Domain.BatchEntity;

public class Batch : Entity
{
    public Guid? SisterBatchId { get; private set; }
    public bool HasSisterBatch => SisterBatchId != null;
    public string DepositorCode { get; private set; }
    public string KitCode { get; private set; }
    public string KitNumber { get; private set; }
    public int KitsCount { get; private set; }
    public int KitsFinished { get; private set; }
    public int KitsLeft => KitsCount - KitsFinished < 0 ? 0 : KitsCount - KitsFinished;
    public string PackagingCode { get; private set; }
    public decimal PackagingHeight { get; private set; }
    public string DefiningPackagingCode { get; private set; }
    public string KitSapDefinitionCode { get; private set; }
    public BatchStatus Status { get; private set; }
    public TimeSpan OptimalKitDuration { get; private set; }
    public TimeSpan OptimalBatchDuration => OptimalKitDuration * KitsCount;
    public TimeSpan OptimalKitsLeftDuration => OptimalKitDuration * KitsLeft;
    public DateTimeOffset? LastWashingActivityAt { get; private set; }
    public DateTimeOffset? WashingStartedAt { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Batch() {}

    public Batch(
        Guid? sisterBatchId,
        string depositorCode,
        string kitCode,
        string kitNumber,
        int kitsCount,
        string packagingCode,
        decimal packagingHeight,
        string definingPackagingCode,
        string kitSapDefinitionCode,
        TimeSpan optimalKitDuration,
        DateTimeOffset? washingStartedAt = null,
        DateTimeOffset? lastWashingActivityAt = null,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(depositorCode))
            throw new ArgumentException("DepositorCode cannot be null or empty.", nameof(depositorCode));
        
        if (string.IsNullOrWhiteSpace(kitCode))
            throw new ArgumentException("KitCode cannot be null or empty.", nameof(kitCode));
        
        if (string.IsNullOrWhiteSpace(kitNumber))
            throw new ArgumentException("KitNumber cannot be null or empty.", nameof(kitNumber));

        if (string.IsNullOrWhiteSpace(packagingCode))
            throw new ArgumentException("PackagingCode cannot be null or empty.", nameof(packagingCode));

        if (string.IsNullOrWhiteSpace(definingPackagingCode))
            throw new ArgumentException("DefiningPackagingCode cannot be null or empty.", nameof(definingPackagingCode));
        
        if (string.IsNullOrWhiteSpace(kitSapDefinitionCode))
            throw new ArgumentException("KitSapDefinitionCode cannot be null or empty.", nameof(kitSapDefinitionCode));
        
        if (kitsCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(kitsCount), "KitsCount must be greater than zero.");

        if (optimalKitDuration <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(optimalKitDuration), "OptimalKitDuration must be greater than zero.");
        
        SisterBatchId = sisterBatchId;
        DepositorCode = depositorCode;
        KitCode = kitCode;
        KitNumber = kitNumber;
        KitsCount = kitsCount;
        PackagingCode = packagingCode;
        PackagingHeight = packagingHeight;
        DefiningPackagingCode = definingPackagingCode;
        KitSapDefinitionCode = kitSapDefinitionCode;
        OptimalKitDuration = optimalKitDuration;
        Status = BatchStatus.InQueue;
        KitsFinished = 0;
        WashingStartedAt = washingStartedAt;
        LastWashingActivityAt = lastWashingActivityAt;
    }
    
    public void FinishKit(DateTimeOffset lastWashingActivityAt)
    {
        KitsFinished++;
        LastWashingActivityAt = lastWashingActivityAt;
    }

    public ErrorOr<Success> CanUpdateKitsCount(int countToChange)
    {
        if (countToChange == 0)
            return BatchErrors.ValidationKitsCountHasToBeGreaterThan0;
        
        if (countToChange - KitsFinished <= 0)
            return BatchErrors.ValidationNotEnoughKitsLeftForDecrease(KitsLeft);
        
        return Result.Success;
    }

    public ErrorOr<Success> UpdateKitsCount(int countToChange)
    {
        var validation = CanUpdateKitsCount(countToChange);
        
        if (validation.IsError)
            return validation.Errors;
        
        KitsCount = countToChange;
        return Result.Success;
    }

    public void StartWashing(DateTimeOffset washingStartedAt)
    {
        Status = BatchStatus.Washing;
        WashingStartedAt = washingStartedAt;
        LastWashingActivityAt = washingStartedAt;
    }
    
    public void SetSisterBatchId(Guid? sisterBatchId)
    {
        SisterBatchId = sisterBatchId;
    }
}