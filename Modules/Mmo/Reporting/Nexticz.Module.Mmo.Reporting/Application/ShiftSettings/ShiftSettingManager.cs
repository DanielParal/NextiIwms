using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.ShiftSettings;

internal class ShiftSettingManager
{
    private readonly List<ShiftSetting> _shifts;
    private readonly DateTime _tenantLocalDateTime;
    private readonly int _hoursBeforeNextShiftShouldBeCreatedConstantValue;

    public ShiftSetting CurrentShift { get; private set; }

    private ShiftSettingManager(DateTime tenantLocalDateTime, int hoursBeforeNextShiftShouldBeCreatedConstantValue, IEnumerable<ShiftSetting> shifts)
    {
        ArgumentNullException.ThrowIfNull(shifts);
        
        var shiftSettings = shifts as ShiftSetting[] ?? shifts.ToArray();
        
        if (shiftSettings.Length == 0)
            throw new InvalidOperationException("No shifts are available.");
        
        _shifts = shiftSettings.OrderBy(s => s.DailyOrder).ToList();
        _tenantLocalDateTime = tenantLocalDateTime;
        _hoursBeforeNextShiftShouldBeCreatedConstantValue = hoursBeforeNextShiftShouldBeCreatedConstantValue;
        SetCurrentShift(TimeOnly.FromDateTime(tenantLocalDateTime), shiftSettings);
    }
    
    internal static ShiftSettingManager Create(DateTime tenantLocalDateTime, int hoursBeforeNextShiftShouldBeCreatedConstantValue, IEnumerable<ShiftSetting> shifts)
    {
        return new ShiftSettingManager(tenantLocalDateTime, hoursBeforeNextShiftShouldBeCreatedConstantValue, shifts);
    }

    private void SetCurrentShift(TimeOnly tenantLocalTime, ShiftSetting[] shiftSettings)
    {
        foreach (var shift in shiftSettings)
        {
            var start = shift.Schedule.StartTimeOnly;
            var end = shift.Schedule.EndTimeOnly;

            if (end > start)
            {
                // Normal shift (e.g., 06:00-13:00)
                if (tenantLocalTime < start || tenantLocalTime >= end) 
                    continue;
                
                CurrentShift = shift;
                return;

            }

            // Shift crossing midnight (e.g., 19:00-06:00)
            if (tenantLocalTime < start && tenantLocalTime >= end) 
                continue;
                
            CurrentShift = shift;
            return;
        }
        
        throw new IndexOutOfRangeException($"Current time is not in any shift schedule. Time: {tenantLocalTime}");
    }
    
    public ShiftSetting GetNextShiftSetting()
    {
        var currentIndex = _shifts.IndexOf(CurrentShift);
        var nextIndex = (currentIndex + 1) % _shifts.Count;
        return _shifts[nextIndex];
    }

    /// <summary>
    /// Determines whether the shift is within the end threshold. End threshold is the number of hours
    /// before the next shift starts. The threshold comes from settings (constants).
    /// </summary>
    public bool IsWithinEndThreshold()
    {
        var today = _tenantLocalDateTime.Date;
        
        var currentShiftEnd = today.Add(CurrentShift.Schedule.EndTimeOnly.ToTimeSpan());
        
        if (currentShiftEnd <= _tenantLocalDateTime)
            currentShiftEnd = currentShiftEnd.AddDays(1);
        
        var hoursDifference = (currentShiftEnd - _tenantLocalDateTime).TotalHours;

        return hoursDifference <= _hoursBeforeNextShiftShouldBeCreatedConstantValue;
    }
    
    public ShiftSchedule GetCurrentShiftSchedule()
    {
        var startDateOnly = DateOnly.FromDateTime(_tenantLocalDateTime);
        var endDateOnly = CurrentShift.Schedule.StartTimeOnly >= CurrentShift.Schedule.EndTimeOnly ?
            startDateOnly.AddDays(1)  // Shift crossing midnight (e.g., 19:00-06:00)
            : startDateOnly;
        
        return new ShiftSchedule(
            startDateOnly.ToDateTime(CurrentShift.Schedule.StartTimeOnly), 
            endDateOnly.ToDateTime(CurrentShift.Schedule.EndTimeOnly));
    }
    
    public ShiftSchedule GetNextShiftSchedule()
    {
        var currentDate = DateOnly.FromDateTime(_tenantLocalDateTime);
        var currentTime = _tenantLocalDateTime.TimeOfDay;

        var nextShiftSetting = GetNextShiftSetting();
        var shiftStart = nextShiftSetting.Schedule.StartTimeOnly;
        var shiftEnd = nextShiftSetting.Schedule.EndTimeOnly;

        DateOnly nextStartDate;

        if (shiftStart < shiftEnd)
        {
            nextStartDate = currentTime < shiftStart.ToTimeSpan() 
                ? currentDate // e.g., Current time is 16:00, shift start is 18:00, so the next shift starts tomorrow
                : currentDate.AddDays(1); // e.g., Current time is 22:00, shift start is 4:00, so the next shift starts tomorrow
        }
        else
        {
            nextStartDate = currentTime >= shiftStart.ToTimeSpan() 
                ? currentDate.AddDays(1) // e.g., Current time is 19:00, shift start is 16:00, and the end is 6:00 - but as we take NEXT shift we need to start tomorrow
                : currentDate; // e.g., Current time is 19:00, shift start is 20:00, and the end is 6:00
        }
        
        var nextEndDate = shiftStart < shiftEnd
            ? nextStartDate
            : nextStartDate.AddDays(1);

        return new ShiftSchedule(
            nextStartDate.ToDateTime(shiftStart),
            nextEndDate.ToDateTime(shiftEnd)
        );
    }
}