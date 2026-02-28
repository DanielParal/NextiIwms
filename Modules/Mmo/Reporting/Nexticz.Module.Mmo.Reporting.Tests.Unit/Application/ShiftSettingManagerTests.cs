using Nexticz.Module.Mmo.Reporting.Application.ShiftSettings;
using Shouldly;

namespace Nexticz.Module.Mmo.Reporting.Tests.Unit.Application;

public class ShiftSettingManagerTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenNoShiftsProvided()
    {
        var now = DateTime.Now;

        var ex = Should.Throw<InvalidOperationException>(() =>
            ShiftSettingManager.Create(now, 6, new List<ShiftSetting>())
        );

        ex.Message.ShouldBe("No shifts are available.");
    }
    
    [Fact]
    public void SetCurrentShift_ShouldSucceed_ForNormalShift()
    {
        var now = new DateTime(2025, 9, 22, 10, 0, 0); // 10:00
        var shifts = new[]
        {
            CreateShiftSetting("Morning", true, 1, "06:00", "14:00"),
            CreateShiftSetting("Afternoon", true, 2, "14:00", "22:00")
        };

        var manager = ShiftSettingManager.Create(now, 6, shifts);

        manager.CurrentShift.ShouldBe(shifts[0]);
    }
    
    [Fact]
    public void SetCurrentShift_ShouldSucceed_ForCrossMidnightShift()
    {
        var now = new DateTime(2025, 9, 22, 23, 30, 0); // 23:30
        var shifts = new[]
        {
            CreateShiftSetting("Morning", true, 1, "22:00", "06:00"),
            CreateShiftSetting("Afternoon", true, 2, "06:00", "14:00")
        };

        var manager = ShiftSettingManager.Create(now, 6, shifts);

        manager.CurrentShift.ShouldBe(shifts[0]);
    }
    
    [Fact]
    public void GetNextShiftSetting_ShouldReturnNextShift()
    {
        var now = new DateTime(2025, 9, 22, 10, 0, 0); // 10:00
        var shifts = new[]
        {
            CreateShiftSetting("Morning", true, 1, "06:00", "14:00"),
            CreateShiftSetting("Afternoon", true, 2, "14:00", "22:00")
        };

        var manager = ShiftSettingManager.Create(now, 6, shifts);

        var nextShift = manager.GetNextShiftSetting();
        nextShift.ShouldBe(shifts[1]);
    }
    
    [Fact]
    public void GetNextShiftSetting_ShouldWrapToFirstShift_WhenAtLastShift()
    {
        var now = new DateTime(2025, 9, 22, 15, 0, 0); // 15:00
        var shifts = new[]
        {
            CreateShiftSetting("Morning", true, 1, "06:00", "14:00"),
            CreateShiftSetting("Afternoon", true, 2, "14:00", "22:00")
        };

        var manager = ShiftSettingManager.Create(now, 6, shifts);

        var nextShift = manager.GetNextShiftSetting();
        nextShift.ShouldBe(shifts[0]);
    }
    
    [Fact]
    public void IsWithinEndThreshold_ShouldReturnTrue_WhenWithinThreshold()
    {
        var now = new DateTime(2025, 9, 22, 13, 0, 0); // 13:00
        var shifts = new[]
        {
            CreateShiftSetting("Morning", true, 1, "06:00", "14:00")
        };

        var manager = ShiftSettingManager.Create(now, 2, shifts);

        var result = manager.IsWithinEndThreshold();
        result.ShouldBeTrue();
    }
    
    [Fact]
    public void IsWithinEndThreshold_ShouldReturnFalse_WhenOutsideThreshold()
    {
        var now = new DateTime(2025, 9, 22, 19, 0, 0); // 19:00
        var shifts = new[]
        {
            CreateShiftSetting("Morning", true, 1, "06:00", "14:00"),
            CreateShiftSetting("Afternoon", true, 2, "14:00", "22:00")
        };

        var manager = ShiftSettingManager.Create(now, 2, shifts);

        var result = manager.IsWithinEndThreshold();
        result.ShouldBeFalse();
    }
    
    [Fact]
    public void IsWithinEndThreshold_ShouldHandleCrossMidnightShift()
    {
        var now = new DateTime(2025, 9, 22, 5, 30, 0); // 05:30
        var shifts = new[]
        {
            CreateShiftSetting("Morning", true, 1, "06:00", "14:00"),
            CreateShiftSetting("Afternoon", true, 2, "22:00", "6:00")
        };

        var manager = ShiftSettingManager.Create(now, 1, shifts);

        var result = manager.IsWithinEndThreshold();
        result.ShouldBeTrue();
    }
    
    [Fact]
    public void GetCurrentShiftSchedule_ShouldReturnCorrectSchedule_ForNormalShift()
    {
        var now = new DateTime(2025, 9, 22, 10, 0, 0); // 10:00
        var shifts = new[]
        {
            CreateShiftSetting("Morning", true, 1, "06:00", "14:00")
        };

        var manager = ShiftSettingManager.Create(now, 6, shifts);

        var schedule = manager.GetCurrentShiftSchedule();

        schedule.Start.ShouldBe(new DateTime(2025, 9, 22, 6, 0, 0));
        schedule.End.ShouldBe(new DateTime(2025, 9, 22, 14, 0, 0));
    }
    
    [Fact]
    public void GetCurrentShiftSchedule_ShouldReturnCorrectSchedule_ForCrossMidnightShift()
    {
        var now = new DateTime(2025, 9, 22, 23, 0, 0); // 23:00
        var shifts = new[]
        {
            CreateShiftSetting("Morning", true, 1, "06:00", "14:00"),
            CreateShiftSetting("Afternoon", true, 2, "22:00", "6:00")
        };

        var manager = ShiftSettingManager.Create(now, 6, shifts);

        var schedule = manager.GetCurrentShiftSchedule();

        schedule.Start.ShouldBe(new DateTime(2025, 9, 22, 22, 0, 0));
        schedule.End.ShouldBe(new DateTime(2025, 9, 23, 6, 0, 0));
    }
    
    [Fact]
    public void GetNextShiftSchedule_ShouldReturnCorrectSchedule_ForNormalShift()
    {
        var now = new DateTime(2025, 9, 22, 10, 0, 0); // 10:00
        var shifts = new[]
        {
            CreateShiftSetting("Morning", true, 1, "06:00", "14:00"),
            CreateShiftSetting("Afternoon", true, 2, "14:00", "22:00")
        };

        var manager = ShiftSettingManager.Create(now, 6, shifts);

        var nextSchedule = manager.GetNextShiftSchedule();

        nextSchedule.Start.ShouldBe(new DateTime(2025, 9, 22, 14, 0, 0));
        nextSchedule.End.ShouldBe(new DateTime(2025, 9, 22, 22, 0, 0));
    }
    
    [Fact]
    public void GetNextShiftSchedule_ShouldReturnCorrectSchedule_ForCrossMidnightShift()
    {
        var now = new DateTime(2025, 9, 22, 23, 0, 0); // 23:00
        var shifts = new[]
        {
            CreateShiftSetting("Morning", true, 1, "22:00", "6:00"),
            CreateShiftSetting("Afternoon", true, 2, "06:00", "14:00")
        };

        var manager = ShiftSettingManager.Create(now, 6, shifts);

        var nextSchedule = manager.GetNextShiftSchedule();

        nextSchedule.Start.ShouldBe(new DateTime(2025, 9, 23, 6, 0, 0));
        nextSchedule.End.ShouldBe(new DateTime(2025, 9, 23, 14, 0, 0));
    }
    
    private static ShiftSetting CreateShiftSetting(string name, bool isActive, int dailyOrder, string start, string end)
    {
        return new ShiftSetting
        {
            Name = name,
            IsActive = isActive,
            DailyOrder = dailyOrder,
            Schedule = new ShiftSettingSchedule
            {
                StartTimeOnly = TimeOnly.Parse(start),
                EndTimeOnly = TimeOnly.Parse(end)
            }
        };
    }
}

