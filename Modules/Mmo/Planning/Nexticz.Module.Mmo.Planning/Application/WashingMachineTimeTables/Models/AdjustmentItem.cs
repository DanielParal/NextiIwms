namespace Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Models;

internal class AdjustmentItem : TimeTableItem
{
    public AdjustmentItem(TimeTableItemType type, DateTimeOffset from, DateTimeOffset to) 
        : base(type, from, to)
    {
    }
}