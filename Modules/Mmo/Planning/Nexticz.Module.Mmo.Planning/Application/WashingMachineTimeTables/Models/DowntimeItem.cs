namespace Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Models;

internal class DowntimeItem : TimeTableItem
{
    public DowntimeItem(TimeTableItemType type, DateTimeOffset from, DateTimeOffset to) 
        : base(type, from, to)
    {
    }
}