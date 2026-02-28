using Nexticz.OnPremise.Monitoring.Exe.DockerMonitoring;
using Nexticz.OnPremise.Monitoring.Exe.ServerMonitoring;
using Nexticz.OnPremise.Monitoring.Exe.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Nexticz.OnPremise.Monitoring.Exe.MonitoringRunner;

internal class MonitoringWorker(
    IDockerMonitoringHandler dockerMonitoringHandler,
    IServerMonitoringHandler serverMonitoringHandler,
    MonitoringRunnerSettings monitoringRunnerSettings,
    ILogger<MonitoringWorker> logger) : BackgroundService
{
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (LogContext.PushProperty("CorrelationId", Base62IdGenerator.GenerateId(7)))
                {
                    await dockerMonitoringHandler.MonitorContainersAsync(stoppingToken);
                    await dockerMonitoringHandler.MonitorDockerDesktopAsync(stoppingToken);
                    serverMonitoringHandler.MonitorServerResources();
                    logger.LogInformation("Monitoring interval completed. Waiting {Interval} before next check.", monitoringRunnerSettings.ExecutionInterval);
                    await Task.Delay(monitoringRunnerSettings.ExecutionInterval, stoppingToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Monitoring worker execution was canceled.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred in monitoring worker.");
            throw;
        }
    }
}