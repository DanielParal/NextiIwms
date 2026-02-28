namespace Nexticz.OnPremise.Monitoring.Exe.Utils;

internal static class ByteFormatter
{
    public static string FormatBytes(ulong bytes)
    {
        string[] suf = ["B", "KB", "MB", "GB", "TB"];
        var idx = 0;
        double val = bytes;
        while (val >= 1024 && idx < suf.Length - 1)
        {
            val /= 1024;
            idx++;
        }
        return $"{val:0.##} {suf[idx]}";
    }
}