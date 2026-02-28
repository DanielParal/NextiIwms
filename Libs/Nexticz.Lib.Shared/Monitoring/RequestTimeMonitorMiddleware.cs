using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Nexticz.Lib.Shared.Monitoring;

internal class RequestTimeMonitorMiddleware(
    RequestDelegate next, MonitoringSettings monitoringSettings, ILogger<RequestTimeMonitorMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();

            var threshold = monitoringSettings.RequestTimeThresholdMilliseconds;
            if (stopwatch.ElapsedMilliseconds > threshold)
            {
                var request = context.Request;
                logger.LogWarning(
                    "Slow request detected: {Method} {Path} took {ElapsedMilliseconds}ms (Threshold: {Threshold}ms)",
                    request.Method,
                    request.Path,
                    stopwatch.ElapsedMilliseconds,
                    threshold
                );
            }
        }
    }
}