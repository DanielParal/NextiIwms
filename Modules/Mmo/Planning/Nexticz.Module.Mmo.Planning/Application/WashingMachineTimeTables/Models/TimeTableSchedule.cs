
namespace Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Models;

internal class TimeTableSchedule
{
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }

    public TimeTableSchedule(DateTimeOffset start, DateTimeOffset end)
    {
        if (start.CompareTo(end) > 0)
            throw new ArgumentException($"Start ({start}) cannot be greater than end ({end}).", nameof(start));
        
        Start = start;
        End = end;
    }
}