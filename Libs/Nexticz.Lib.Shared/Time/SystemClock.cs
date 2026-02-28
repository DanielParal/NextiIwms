using Microsoft.Extensions.Options;

namespace Nexticz.Lib.Shared.Time;

public class SystemClock(IOptions<TimeZoneSettings> timeZoneSettings) : IClock
{
    private readonly TimeZoneSettings _timeZoneSettings = timeZoneSettings.Value;
    
    public string GetTenantTimeZoneId => _timeZoneSettings.TenantTimeZoneId;
    public TimeZoneInfo? GetTimeZoneInfoById(string timeZoneId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException e)
        {
            return null;
        }
    }

    public DateTimeOffset GetTodayWithTenantTime(TimeOnly timeOnly)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_timeZoneSettings.TenantTimeZoneId);
        var today = DateTime.Today;
        var dateTime = today.Add(timeOnly.ToTimeSpan());
        return new DateTimeOffset(dateTime, timeZone.GetUtcOffset(dateTime));
    }

    public DateTimeOffset UtcNowOffset => TruncateToMilliseconds(DateTimeOffset.UtcNow);
    public DateTimeOffset TenantNowOffset
    {
        get
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_timeZoneSettings.TenantTimeZoneId);
            return TruncateToMilliseconds(TimeZoneInfo.ConvertTime(UtcNowOffset, timeZone));
        }
    }

    // If we need to convert utc time to tenant time and display the same date time in all zones we can use this method
    public DateTime ConvertUtcToTenantDateTime(DateTimeOffset utcDateOffset)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_timeZoneSettings.TenantTimeZoneId);
        var tenantDateTime = TimeZoneInfo.ConvertTime(utcDateOffset.DateTime, timeZone);
        return DateTime.SpecifyKind(tenantDateTime, DateTimeKind.Unspecified);
    }

    public DateTime ConvertTenantToUtcDateTime(DateTimeOffset tenantDateOffset)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_timeZoneSettings.TenantTimeZoneId);
        var utcDateTime = TimeZoneInfo.ConvertTime(tenantDateOffset.DateTime, timeZone, TimeZoneInfo.Utc);
        return DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
    }

    /// <summary>
    /// This function is useful when we need to store date in UTC and we want to normalize it to noon so it is the same date in all time zones.
    /// We use it when we need to use from and to date (not time)
    /// </summary>
    public DateTimeOffset NormalizeToNoonUtc(DateTimeOffset inputUtc)
    {
        var localDate = inputUtc.Date;
        var utcNoon = localDate.AddHours(12);
        return new DateTimeOffset(utcNoon, TimeSpan.Zero);
    }

    public DateOnly GetTenantDateOnly(DateTimeOffset utcDateTimeOffset)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_timeZoneSettings.TenantTimeZoneId);
        var tenantDateTime = TimeZoneInfo.ConvertTime(utcDateTimeOffset, timeZone);
        return DateOnly.FromDateTime(tenantDateTime.DateTime);
    }

    private static DateTimeOffset TruncateToMilliseconds(DateTimeOffset dt)
    {
        var ticks = dt.Ticks - (dt.Ticks % TimeSpan.TicksPerMillisecond);
        return new DateTimeOffset(ticks, dt.Offset);
    }
}