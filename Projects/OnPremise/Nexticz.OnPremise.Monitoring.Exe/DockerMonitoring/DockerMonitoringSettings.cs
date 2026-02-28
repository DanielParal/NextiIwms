namespace Nexticz.OnPremise.Monitoring.Exe.DockerMonitoring;

internal class DockerMonitoringSettings
{
    public string SuccessMessageForCloudAlerts { get; set; }
    public string DockerUri { get; set; }
    public string[] ContainerNames { get; set; }
    public string DockerDesktopProcessName { get; set; }
}