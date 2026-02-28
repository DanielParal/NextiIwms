using Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Models;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Models;

internal abstract class TimeTableItem
{
    public TimeTableItemType Type { get; set; }
    public TimeTableSchedule TimeTableSchedule { get; set; }
    
    public TimeTableItem(TimeTableItemType type, DateTimeOffset from, DateTimeOffset to)
    {
        Type = type;
        TimeTableSchedule = new TimeTableSchedule(from, to);
    }
}