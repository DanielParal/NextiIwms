namespace Nexticz.Lib.Shared.Extensions;

public static class TimeSpanExtension
{
    public static string ToHhMmSsString(this TimeSpan timeSpan)
    {
        return timeSpan.ToString(@"hh\:mm\:ss");
    }
}