namespace Nexticz.Module.Mmo.Reporting.Application.BreakSettings;

internal class BreakSetting
{
    public TimeOnly StartTimeOnly { get; set; }
    public TimeOnly EndTimeOnly { get; set; }
    
    public (DateTimeOffset Start, DateTimeOffset End) GetBreakTimeRange(DateTimeOffset shiftStartDate) 
    {
        var shiftStartTime = TimeOnly.FromDateTime(shiftStartDate.LocalDateTime);
        var shiftStartDateOnly = DateOnly.FromDateTime(shiftStartDate.LocalDateTime);
        
        var start = StartTimeOnly < shiftStartTime && EndTimeOnly < shiftStartTime ? shiftStartDateOnly.ToDateTime(StartTimeOnly).AddDays(1) : shiftStartDateOnly.ToDateTime(StartTimeOnly);
        var end = EndTimeOnly > shiftStartTime ? shiftStartDateOnly.ToDateTime(EndTimeOnly) : shiftStartDateOnly.ToDateTime(EndTimeOnly).AddDays(1);
         
         return (start, end);
    }
}