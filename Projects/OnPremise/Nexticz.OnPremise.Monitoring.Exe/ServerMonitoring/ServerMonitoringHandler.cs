using System.Text;
using Hardware.Info;
using Nexticz.OnPremise.Monitoring.Exe.Utils;
using Microsoft.Extensions.Logging;

namespace Nexticz.OnPremise.Monitoring.Exe.ServerMonitoring;

internal class ServerMonitoringHandler(
    ILogger<ServerMonitoringHandler> logger) : IServerMonitoringHandler
{
    public void MonitorServerResources()
    {
        try
        {
            var hardwareInfo = new HardwareInfo();
        
            hardwareInfo.RefreshMemoryStatus();
        
            var totalMemory = hardwareInfo.MemoryStatus.TotalPhysical;
            var availableMemory = hardwareInfo.MemoryStatus.AvailablePhysical;
            var usedMemory = totalMemory - availableMemory;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Server resources");
            stringBuilder.AppendLine($"Total RAM: {ByteFormatter.FormatBytes(totalMemory)}");
            stringBuilder.AppendLine($"Used RAM: {ByteFormatter.FormatBytes(usedMemory)}");
            stringBuilder.AppendLine($"Available RAM: {ByteFormatter.FormatBytes(availableMemory)}");
        
            hardwareInfo.RefreshCPUList(true);

            var totalCpuUsage = 0.0;
            foreach (var cpu in hardwareInfo.CpuList)
            {
                totalCpuUsage += cpu.PercentProcessorTime;
            }

            // Average CPU load across all cores
            totalCpuUsage /= hardwareInfo.CpuList.Count;

            stringBuilder.AppendLine($"Total CPU Usage: {totalCpuUsage:0.##}%");
        
            logger.LogInformation(stringBuilder.ToString());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting server resources info: {ErrorMessage}", ex.Message);
        }
    }
}