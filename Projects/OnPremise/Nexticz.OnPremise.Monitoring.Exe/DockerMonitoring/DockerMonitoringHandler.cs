using System.Diagnostics;
using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;
using Nexticz.OnPremise.Monitoring.Exe.Utils;
using Microsoft.Extensions.Logging;

namespace Nexticz.OnPremise.Monitoring.Exe.DockerMonitoring;

internal class DockerMonitoringHandler(
    DockerMonitoringSettings dockerMonitoringSettings,
    ILogger<DockerMonitoringHandler> logger) : IDockerMonitoringHandler
{
    public async Task MonitorContainersAsync(CancellationToken cancellationToken)
    {
        // Configure Docker client (local Docker daemon)
        using var dockerClient = new DockerClientConfiguration(
            new Uri(dockerMonitoringSettings.DockerUri)
        ).CreateClient();

        // Get list of all containers (running and stopped)
        var containers = await dockerClient.Containers.ListContainersAsync(
            new ContainersListParameters
            {
                All = true
            }, cancellationToken);
            
        // Filter containers by names from DockerConfig
        var filteredContainers = 
            containers.Where(c => 
                    c.Names.Any(name => dockerMonitoringSettings.ContainerNames.Any(configName =>
                        name.Equals($"/{configName}", StringComparison.OrdinalIgnoreCase))))
                .ToArray();
            
        // Display container details
        if (filteredContainers.Length == 0)
        {
            logger.LogCritical("Container Monitoring - No containers found!");
            return;
        }

        if (filteredContainers.Length != dockerMonitoringSettings.ContainerNames.Length)
        {
            logger.LogCritical("Container Monitoring - we found only {FilteredContainers} out of {ContainersFromConfig}!", 
                filteredContainers.Length, dockerMonitoringSettings.ContainerNames.Length);
            return;
        }
        
        var notRunningContainers = filteredContainers.Where(c => c.State != "running").ToArray();

        if (notRunningContainers.Length == 0)
        {
            // We cannot change this text because it is used in Azure for docker monitoring. The rule searches for this text to make sure everything is working.
            logger.LogInformation(dockerMonitoringSettings.SuccessMessageForCloudAlerts);
            return;
        }
        
        foreach (var container in notRunningContainers)
        {
            logger.LogCritical("Container Monitoring - container is not running! Container ID: {ContainerId}, Image: {ContainerImage}, " +
                                  "Status: {ContainerStatus}, State: {ContainerState}, Created: {ContainerCreated}",
                container.ID, container.Image, container.Status, container.State, container.Created.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }

    public async Task MonitorDockerDesktopAsync(CancellationToken cancellationToken)
    {
        var processName = dockerMonitoringSettings.DockerDesktopProcessName;
        try
        {
            var processes = Process.GetProcessesByName(processName);
            
            if (processes.Length == 0)
            {
                logger.LogError("Docker Monitoring - No processes with name: {ProcessName} found.", processName);
                return;
            }
            
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Docker Desktop Monitoring");
            
            foreach (var proc in processes)
            {
                var cpuUsage = await CpuMonitor.SampleCpuUsageAsync(proc, token: cancellationToken);
                stringBuilder.AppendLine($"PID: {proc.Id}");
                stringBuilder.AppendLine($"Name: {proc.ProcessName}");
                stringBuilder.AppendLine($"CPU Usage: {cpuUsage}%");
                stringBuilder.AppendLine($"Memory: {proc.WorkingSet64 / 1024 / 1024} MB");
                stringBuilder.AppendLine();
            }
            
            logger.LogInformation(stringBuilder.ToString());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Docker Monitoring - Error getting info about process: {ProcessName}.", processName);
        }
    }
    
    
}