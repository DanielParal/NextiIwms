using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nexticz.OnPremise.Monitoring.Exe.DockerMonitoring;

internal static class DockerMonitoringRegistrator
{
    public static IServiceCollection AddDockerMonitoring(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDockerMonitoringHandler, DockerMonitoringHandler>();
        
        var monitoringSettings = configuration.GetSection(nameof(DockerMonitoringSettings)).Get<DockerMonitoringSettings>()
                                 ?? throw new InvalidOperationException("Docker monitoring settings section is missing.");

        services.AddSingleton(monitoringSettings);

        return services;
    }
}