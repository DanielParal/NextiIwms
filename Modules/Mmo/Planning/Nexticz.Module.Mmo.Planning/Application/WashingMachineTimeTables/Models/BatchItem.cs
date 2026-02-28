using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Models;

internal class BatchItem : TimeTableItem
{
    public Guid Id { get; set; }
    public Guid? SisterBatchId { get; set; }
    public string KitCode { get; set; }
    public string PackagingCode { get; set; }
    public decimal PackagingHeight { get; set; }
    public string? SisterPackagingCode { get; set; }
    public int KitFinished { get; set; }
    public int KitsCount { get; set; }
    public TimeSpan OptimalKitDuration { get; set; }
    public int Index { get; set; }
    public int? SisterBatchIndex { get; set; }
    public BatchStatus Status { get; set; }
    
    public BatchItem(
        Guid id,
        TimeTableItemType type, 
        Guid? sisterBatchId,
        DateTimeOffset from, 
        DateTimeOffset to, 
        string kitCode, 
        string packagingCode, 
        decimal packagingHeight, 
        string? sisterPackagingCode, 
        int kitFinished, 
        int kitsCount, 
        TimeSpan optimalKitDuration,
        int index,
        int? sisterBatchIndex,
        BatchStatus status) 
        : base(type, from, to)
    {
        Id = id;
        SisterBatchId = sisterBatchId;
        KitCode = kitCode;
        PackagingCode = packagingCode;
        PackagingHeight = packagingHeight;
        SisterPackagingCode = sisterPackagingCode;
        KitFinished = kitFinished;
        KitsCount = kitsCount;
        OptimalKitDuration = optimalKitDuration;
        Index = index;
        SisterBatchIndex = sisterBatchIndex;
        Status = status;
    }
}