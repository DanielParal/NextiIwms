using Nexticz.Module.Mmo.Reporting.Application.BreakSettings;
using Shouldly;

namespace Nexticz.Module.Mmo.Reporting.Tests.Unit.Application;

public class BreakSettingTests
{
    [Theory]
    [InlineData("2025-06-25 08:00", "09:00", "10:00", "2025-06-25 09:00", "2025-06-25 10:00")] // Normal same-day break
    [InlineData("2025-06-25 23:00", "01:00", "02:00", "2025-06-26 01:00", "2025-06-26 02:00")] // Break after midnight
    [InlineData("2025-06-25 01:00", "23:00", "01:00", "2025-06-25 23:00", "2025-06-26 01:00")] // Break spans midnight
    [InlineData("2025-06-25 10:00", "08:00", "09:00", "2025-06-26 08:00", "2025-06-26 09:00")] // Break both start & end before shift, so moves to next day
    [InlineData("2025-06-25 03:00", "02:00", "04:00", "2025-06-25 02:00", "2025-06-25 04:00")] // Mixed condition
    public void GetBreakTimeRange_WhenCalled_ShouldReturnCorrectTimeRange(string shiftStartStr, string breakStartStr, string breakEndStr, string expectedStartStr, string expectedEndStr)
    {
        // Arrange
        var shiftStart = DateTimeOffset.Parse(shiftStartStr);
        var expectedStart = DateTimeOffset.Parse(expectedStartStr);
        var expectedEnd = DateTimeOffset.Parse(expectedEndStr);

        var breakSetting = new BreakSetting
        {
            StartTimeOnly = TimeOnly.Parse(breakStartStr),
            EndTimeOnly = TimeOnly.Parse(breakEndStr)
        };

        // Act
        var (actualStart, actualEnd) = breakSetting.GetBreakTimeRange(shiftStart);

        // Assert
        actualStart.ShouldBe(expectedStart);
        actualEnd.ShouldBe(expectedEnd);
    }
}