using System.Diagnostics;

namespace Nexticz.OnPremise.Monitoring.Exe.Utils;

internal static class CpuMonitor
{
    public static async Task<double> SampleCpuUsageAsync(Process process, int delayMs = 500, CancellationToken token = default)
    {
        var processorCount = Environment.ProcessorCount;

        // Take first snapshot
        var startCpu = process.TotalProcessorTime.TotalMilliseconds;
        var startTime = DateTime.UtcNow;

        await Task.Delay(delayMs, token);

        // Take second snapshot
        var endCpu = process.TotalProcessorTime.TotalMilliseconds;
        var endTime = DateTime.UtcNow;
        
        var cpuUsedMs = endCpu - startCpu;
        var totalMsPassed = (endTime - startTime).TotalMilliseconds;

        return Math.Round((cpuUsedMs / (totalMsPassed * processorCount)) * 100, 2);
    }
}