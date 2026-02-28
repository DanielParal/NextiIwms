using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nexticz.Lib.Shared.Monitoring;

public static class MonitoringRegistrator
{
    public static IServiceCollection AddMonitoringConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var monitoringSettings = configuration.GetSection(nameof(MonitoringSettings)).Get<MonitoringSettings>()
                                 ?? throw new InvalidOperationException("Monitor settings section is missing.");

        services.AddSingleton(monitoringSettings);
        
        return services;
    }
    
    public static void UseMonitoringConfiguration(this WebApplication app)
    {
        app.UseMiddleware<RequestTimeMonitorMiddleware>();
    }
}