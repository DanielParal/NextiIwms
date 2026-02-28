using Nexticz.Module.Mmo.Reporting.Application.Shifts;
using Nexticz.Module.Mmo.Reporting.Application.ShiftSettings;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;
using Shouldly;

namespace Nexticz.Module.Mmo.Reporting.Tests.Unit.Application;

public class ShiftCreationDeciderTests
{
    
    [Fact]
    public void ShouldNotCreateShift_WhenLastShiftStillRunning()
    {
        var now = new DateTimeOffset(2025, 9, 22, 10, 0, 0, TimeSpan.FromHours(2)); // Tenant: 10:00
        var lastShift = CreateShift(
            start: now.AddHours(-1),
            end: now.AddHours(2)
        );

        var shifts = new[]
        {
            CreateShiftSetting("Morning", true, 1, "06:00", "14:00")
        };

        var manager = ShiftSettingManager.Create(now.LocalDateTime, 6, shifts);
        var result = ShiftCreationDecider.ShouldCreateNewShift(lastShift, manager, now);

        result.ShouldCreate.ShouldBeFalse();
        result.NextShiftName.ShouldBeNull();
        result.NextShiftSchedule.ShouldBeNull();
    }
    
    [Fact]
    public void ShouldCreateNextShift_WhenCurrentShiftActive_AndWithinThreshold()
    {
        var now = new DateTimeOffset(2025, 9, 22, 15, 0, 0, TimeSpan.FromHours(2)); // Tenant: 15:00
        var lastShift = CreateShift(
            start: new DateTimeOffset(2025, 9, 22, 6, 0, 0, TimeSpan.FromHours(2)),
            end: new DateTimeOffset(2025, 9, 22, 14, 0, 0, TimeSpan.FromHours(2))
        );

        var shifts = new[]
        {
            CreateShiftSetting("Morning", true, 1, "06:00", "14:00"),
            CreateShiftSetting("Evening", true, 2, "14:00", "22:00")
        };

        var manager = ShiftSettingManager.Create(now.LocalDateTime, 6, shifts);
        var result = ShiftCreationDecider.ShouldCreateNewShift(lastShift, manager, now);

        result.ShouldCreate.ShouldBeTrue();
        result.NextShiftName.ShouldBe("Evening");
        result.NextShiftSchedule.ShouldNotBeNull();
        result.NextShiftSchedule.Start.ShouldBe(new DateTime(2025, 9, 22, 14, 0, 0));
        result.NextShiftSchedule.End.ShouldBe(new DateTime(2025, 9, 22, 22, 0, 0));
    }
    
    [Fact]
    public void ShouldCreateCurrentShift_WhenCurrentShiftActive_AndOutsideThreshold()
    {
        var now = new DateTimeOffset(2025, 9, 22, 10, 0, 0, TimeSpan.FromHours(2)); // Tenant: 10:00
        var lastShift = CreateShift(
            start: new DateTimeOffset(2025, 9, 21, 6, 0, 0, TimeSpan.FromHours(2)),
            end: new DateTimeOffset(2025, 9, 21, 14, 0, 0, TimeSpan.FromHours(2))
        );

        var shifts = new[]
        {
            CreateShiftSetting("Morning", true, 1, "06:00", "18:00"),
            CreateShiftSetting("Evening", true, 2, "18:00", "23:00")
        };

        var manager = ShiftSettingManager.Create(now.LocalDateTime, 6, shifts);
        var result = ShiftCreationDecider.ShouldCreateNewShift(lastShift, manager, now);

        result.ShouldCreate.ShouldBeTrue();
        result.NextShiftName.ShouldBe("Morning");
        result.NextShiftSchedule.ShouldNotBeNull();
        result.NextShiftSchedule.Start.ShouldBe(new DateTime(2025, 9, 22, 6, 0, 0));
        result.NextShiftSchedule.End.ShouldBe(new DateTime(2025, 9, 22, 18, 0, 0));
    }
    
    [Fact]
    public void ShouldNotCreateShift_WhenCurrentShiftInactive_AndOutsideThreshold()
    {
        var now = new DateTimeOffset(2025, 9, 22, 7, 0, 0, TimeSpan.FromHours(2));
        var lastShift = CreateShift(
            start: new DateTimeOffset(2025, 9, 21, 14, 0, 0, TimeSpan.FromHours(2)),
            end: new DateTimeOffset(2025, 9, 21, 22, 0, 0, TimeSpan.FromHours(2))
        );

        var shifts = new[]
        {
            CreateShiftSetting("Morning", false, 1, "06:00", "14:00"),
            CreateShiftSetting("Evening", true, 2, "14:00", "22:00")
        };

        var manager = ShiftSettingManager.Create(now.LocalDateTime, 6, shifts);
        var result = ShiftCreationDecider.ShouldCreateNewShift(lastShift, manager, now);

        result.ShouldCreate.ShouldBeFalse();
        result.NextShiftName.ShouldBeNull();
        result.NextShiftSchedule.ShouldBeNull();
    }
    
    [Fact]
    public void ShouldCreateNextShift_WhenCurrentShiftInactive_AndWithinThreshold()
    {
        var now = new DateTimeOffset(2025, 9, 22, 13, 30, 0, TimeSpan.FromHours(2)); // 13:30
        var lastShift = CreateShift(
            start: new DateTimeOffset(2025, 9, 21, 14, 0, 0, TimeSpan.FromHours(2)),
            end: new DateTimeOffset(2025, 9, 21, 22, 0, 0, TimeSpan.FromHours(2))
        );

        var shifts = new[]
        {
            CreateShiftSetting("Morning", false, 1, "06:00", "14:00"),
            CreateShiftSetting("Evening", true, 2, "14:00", "22:00")
        };

        var manager = ShiftSettingManager.Create(now.LocalDateTime, 6, shifts);
        var result = ShiftCreationDecider.ShouldCreateNewShift(lastShift, manager, now);

        result.ShouldCreate.ShouldBeTrue();
        result.NextShiftName.ShouldBe("Evening");
        result.NextShiftSchedule.ShouldNotBeNull();
        result.NextShiftSchedule.Start.ShouldBe(new DateTime(2025, 9, 22, 14, 0, 0));
        result.NextShiftSchedule.End.ShouldBe(new DateTime(2025, 9, 22, 22, 0, 0));
    }
    
    [Fact]
    public void ShouldCreateNextShift_ForCrossMidnightShift()
    {
        var now = new DateTimeOffset(2025, 9, 22, 23, 30, 0, TimeSpan.FromHours(2));
        var lastShift = CreateShift(
            start: new DateTimeOffset(2025, 9, 21, 14, 0, 0, TimeSpan.FromHours(2)),
            end: new DateTimeOffset(2025, 9, 22, 1, 0, 0, TimeSpan.FromHours(2))
        );

        var shifts = new[]
        {
            CreateShiftSetting("Night", false, 1, "14:00", "01:00"),
            CreateShiftSetting("Morning", true, 2, "01:00", "08:00")
        };

        var manager = ShiftSettingManager.Create(now.LocalDateTime, 6, shifts);
        var result = ShiftCreationDecider.ShouldCreateNewShift(lastShift, manager, now);

        result.ShouldCreate.ShouldBeTrue();
        result.NextShiftName.ShouldBe("Morning");
        result.NextShiftSchedule.ShouldNotBeNull();
        result.NextShiftSchedule.Start.ShouldBe(new DateTime(2025, 9, 23, 1, 0, 0));
        result.NextShiftSchedule.End.ShouldBe(new DateTime(2025, 9, 23, 8, 0, 0));
    }
    
    [Fact]
    public void ShouldCreateNextShift_WhenLastShiftIsNull()
    {
        var now = new DateTimeOffset(2025, 9, 22, 7, 30, 0, TimeSpan.FromHours(2));
        Shift? lastShift = null;

        var shifts = new[]
        {
            CreateShiftSetting("Morning", true, 1, "06:00", "18:00"),
            CreateShiftSetting("Evening", true, 2, "18:00", "22:00")
        };

        var manager = ShiftSettingManager.Create(now.LocalDateTime, 6, shifts);
        var result = ShiftCreationDecider.ShouldCreateNewShift(lastShift, manager, now);

        result.ShouldCreate.ShouldBeTrue();
        result.NextShiftName.ShouldBe("Morning");
        result.NextShiftSchedule.ShouldNotBeNull();
        result.NextShiftSchedule.Start.ShouldBe(new DateTimeOffset(2025, 9, 22, 6, 0, 0, TimeSpan.FromHours(2)));
        result.NextShiftSchedule.End.ShouldBe(new DateTimeOffset(2025, 9, 22, 18, 0, 0, TimeSpan.FromHours(2)));
    }
    
    [Fact]
    public void ShouldNotCreateShift_WhenLastShiftIsStillValid()
    {
        var now = new DateTimeOffset(2025, 9, 22, 10, 0, 0, TimeSpan.FromHours(2));
        var lastShift = CreateShift(
            start: new DateTimeOffset(2025, 9, 22, 6, 0, 0, TimeSpan.FromHours(2)),
            end: new DateTimeOffset(2025, 9, 22, 14, 0, 0, TimeSpan.FromHours(2))
        );

        var shifts = new[]
        {
            CreateShiftSetting("Morning", false, 1, "06:00", "14:00"),
            CreateShiftSetting("Evening", true, 2, "14:00", "22:00")
        };

        var manager = ShiftSettingManager.Create(now.LocalDateTime, 6, shifts);
        var result = ShiftCreationDecider.ShouldCreateNewShift(lastShift, manager, now);

        result.ShouldCreate.ShouldBeFalse();
        result.NextShiftName.ShouldBeNull();
        result.NextShiftSchedule.ShouldBeNull();
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

    private static Shift CreateShift(DateTimeOffset start, DateTimeOffset end)
    {
        return new Shift(
            Guid.NewGuid().ToString(),
            true,
            false,
            new ShiftSchedule(start.DateTime, end.DateTime),
            DateTimeOffset.UtcNow,
            [],
            Guid.NewGuid());
    }
}