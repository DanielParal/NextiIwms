namespace Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Models;

internal class TimeTableQueue
{
    public string Code { get; init; }
    public bool IsActive { get; init; }
    public List<TimeTableItem> Items { get; init; } = [];
    
    public DateTimeOffset? GetLastEndDate()
    {
        return HasItems ? Items.Last().TimeTableSchedule.End : null;
    }
    
    public BatchItem? GetLastBatchItem => BatchCount > 0 ? (BatchItem)Items.Last(x => x.Type == TimeTableItemType.Batch) : null;
    public int BatchCount => Items.Count(x => x.Type == TimeTableItemType.Batch);
    public bool HasItems => Items.Count > 0;
}