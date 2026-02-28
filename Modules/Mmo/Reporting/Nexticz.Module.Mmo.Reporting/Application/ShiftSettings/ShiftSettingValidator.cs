namespace Nexticz.Module.Mmo.Reporting.Application.ShiftSettings;

internal static class ShiftSettingValidator
{
    public static bool ValidateShifts(ShiftSetting[] shifts)
    {
        if (shifts.Length == 0)
            return false;

        // Check if shifts cover exactly 24 hours
        var totalDuration = TimeSpan.Zero;
        foreach (var shift in shifts)
        {
            totalDuration += shift.Schedule.EndTimeOnly <= shift.Schedule.StartTimeOnly
                ? (TimeOnly.MaxValue.ToTimeSpan() + TimeSpan.FromTicks(1)) - shift.Schedule.StartTimeOnly.ToTimeSpan() + shift.Schedule.EndTimeOnly.ToTimeSpan()
                : shift.Schedule.EndTimeOnly - shift.Schedule.StartTimeOnly;
        }
        
        if (totalDuration != TimeSpan.FromHours(24))
            return false;

        // Sort shifts by start hour
        var sortedShifts = shifts.OrderBy(s => s.Schedule.StartTimeOnly).ToList();

        // Check for overlaps and gaps
        ShiftSetting? previousShift = null;
        foreach (var currentShift in sortedShifts)
        {
            // Validate time range (ensure within 00:00 to 23:59)
            if (currentShift.Schedule.StartTimeOnly < TimeOnly.MinValue || currentShift.Schedule.StartTimeOnly > TimeOnly.MaxValue ||
                currentShift.Schedule.EndTimeOnly < TimeOnly.MinValue || currentShift.Schedule.EndTimeOnly > TimeOnly.MaxValue)
                return false;

            // Check if current shift's end matches next shift's start
            if (previousShift is not null &&
                currentShift.Schedule.StartTimeOnly != previousShift.Schedule.EndTimeOnly)
                return false;
            
            previousShift = currentShift;
        }

        return true;
    }
}