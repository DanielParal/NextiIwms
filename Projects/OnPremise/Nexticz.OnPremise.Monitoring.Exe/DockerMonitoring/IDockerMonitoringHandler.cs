namespace Nexticz.OnPremise.Monitoring.Exe.DockerMonitoring;

internal interface IDockerMonitoringHandler
{
    Task MonitorContainersAsync(CancellationToken cancellationToken);
    Task MonitorDockerDesktopAsync(CancellationToken cancellationToken);
}