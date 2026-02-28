using Nexticz.Module.Mmo.Reporting.Application.ShiftSettings;
using Nexticz.Module.Mmo.Reporting.Application.ShiftSettings.Queries.GetShiftSettings;
using Shouldly;

namespace Nexticz.Module.Mmo.Reporting.Tests.Unit.Application;

public class ShiftSettingValidatorTests
{
    [Fact]
    public void ValidateShifts_ValidShifts_ReturnsTrue()
    {
        // Arrange
        var shifts = new List<ShiftSetting>
        {
            new() { Schedule = new ShiftSettingSchedule { StartTimeOnly = new TimeOnly(6, 0), EndTimeOnly = new TimeOnly(13, 0) } },
            new() { Schedule = new ShiftSettingSchedule { StartTimeOnly = new TimeOnly(13, 0), EndTimeOnly = new TimeOnly(19, 0) } },
            new() { Schedule = new ShiftSettingSchedule { StartTimeOnly = new TimeOnly(19, 0), EndTimeOnly = new TimeOnly(6, 0) } }
        };

        // Act
        var result = ShiftSettingValidator.ValidateShifts(shifts.ToArray());

        // Assert
        result.ShouldBeTrue();
    }
    
    [Fact]
    public void ValidateShifts_NotValidShiftsNotFollowEachOther_ReturnsFalse()
    {
        // Arrange
        var shifts = new List<ShiftSetting>
        {
            new() { Schedule = new ShiftSettingSchedule { StartTimeOnly = new TimeOnly(6, 0), EndTimeOnly = new TimeOnly(12, 0) } },
            new() { Schedule = new ShiftSettingSchedule { StartTimeOnly = new TimeOnly(14, 0), EndTimeOnly = new TimeOnly(18, 0) } },
            new() { Schedule = new ShiftSettingSchedule { StartTimeOnly = new TimeOnly(19, 0), EndTimeOnly = new TimeOnly(6, 0) } }
        };

        // Act
        var result = ShiftSettingValidator.ValidateShifts(shifts.ToArray());

        // Assert
        result.ShouldBeFalse();
    }
    
    [Fact]
    public void ValidateShifts_NotValidShiftsOverlapEachOther_ReturnsFalse()
    {
        // Arrange
        var shifts = new List<ShiftSetting>
        {
            new() { Schedule = new ShiftSettingSchedule { StartTimeOnly = new TimeOnly(6, 0), EndTimeOnly = new TimeOnly(14, 0) } },
            new() { Schedule = new ShiftSettingSchedule { StartTimeOnly = new TimeOnly(13, 0), EndTimeOnly = new TimeOnly(22, 0) } },
            new() { Schedule = new ShiftSettingSchedule { StartTimeOnly = new TimeOnly(19, 0), EndTimeOnly = new TimeOnly(6, 0) } }
        };

        // Act
        var result = ShiftSettingValidator.ValidateShifts(shifts.ToArray());

        // Assert
        result.ShouldBeFalse();
    }
    
    [Fact]
    public void ValidateShifts_NotValidShiftsCorrectHoursButNotFollowEachOther_ReturnsFalse()
    {
        // Arrange
        var shifts = new List<ShiftSetting>
        {
            new() { Schedule = new ShiftSettingSchedule { StartTimeOnly = new TimeOnly(5, 0), EndTimeOnly = new TimeOnly(13, 0) } },
            new() { Schedule = new ShiftSettingSchedule { StartTimeOnly = new TimeOnly(11, 0), EndTimeOnly = new TimeOnly(19, 0) } },
            new() { Schedule = new ShiftSettingSchedule { StartTimeOnly = new TimeOnly(19, 0), EndTimeOnly = new TimeOnly(7, 0) } }
        };

        // Act
        var result = ShiftSettingValidator.ValidateShifts(shifts.ToArray());

        // Assert
        result.ShouldBeFalse();
    }
    
    [Fact]
    public void ValidateShifts_ValidOneShift_ReturnsTrue()
    {
        // Arrange
        var shifts = new List<ShiftSetting>
        {
            new() { Schedule = new ShiftSettingSchedule { StartTimeOnly = TimeOnly.MinValue, EndTimeOnly = TimeOnly.MinValue } }
        };

        // Act
        var result = ShiftSettingValidator.ValidateShifts(shifts.ToArray());

        // Assert
        result.ShouldBeTrue();
    }
}