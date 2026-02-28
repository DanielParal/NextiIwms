using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nexticz.OnPremise.Monitoring.Exe.MonitoringRunner;

internal static class MonitoringRunnerRegistrator
{
    public static IServiceCollection AddMonitoringRunner(this IServiceCollection services, IConfiguration configuration)
    {
        var monitoringRunnerSettings = configuration.GetSection(nameof(MonitoringRunnerSettings)).Get<MonitoringRunnerSettings>()
                                 ?? throw new InvalidOperationException("Monitoring runner settings section is missing.");

        services.AddSingleton(monitoringRunnerSettings);
        
        services.AddHostedService<MonitoringWorker>();

        return services;
    }
}