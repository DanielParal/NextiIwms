namespace Nexticz.Lib.Shared.Time;

public interface IClock
{
    DateTimeOffset UtcNowOffset { get; }
    DateTimeOffset TenantNowOffset { get; }
    DateTime ConvertUtcToTenantDateTime(DateTimeOffset utcDateOffset);
    DateTime ConvertTenantToUtcDateTime(DateTimeOffset tenantDateOffset);
    DateTimeOffset NormalizeToNoonUtc(DateTimeOffset inputUtc);
    string GetTenantTimeZoneId { get; }
    TimeZoneInfo? GetTimeZoneInfoById(string timeZoneId);
    DateTimeOffset GetTodayWithTenantTime(TimeOnly timeOnly);
    DateOnly GetTenantDateOnly(DateTimeOffset utcDateTimeOffset);
}