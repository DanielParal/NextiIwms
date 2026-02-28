using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nexticz.Module.Mmo.Drying;
using Nexticz.Module.Mmo.Planning;
using Nexticz.Module.Mmo.Reporting;
using Nexticz.Module.Mmo.Settings;
using Nexticz.Module.Mmo.Washing;

namespace Nexticz.Module.Mmo.ModuleRegistrator;

public static class ModuleRegistrator
{
    public static IHealthChecksBuilder AddMmoHealthChecks(this IHealthChecksBuilder builder, IConfiguration configuration)
    {
        var dbConnectionString = configuration.GetConnectionString("Mmo");
        builder.AddNpgSql(
            connectionString: dbConnectionString!,
            name: "Mmo db - module database",
            failureStatus: HealthStatus.Unhealthy,
            tags: ["module", "mmo", "db", "postgres"]);
        
        return builder;
    }
    
    public static IServiceCollection AddMmoModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMmoSettingsModule(configuration)
            .AddMmoPlanningModule(configuration)
            .AddMmoWashingModule(configuration)
            .AddMmoDryingModule(configuration)
            .AddMmoReportingModule(configuration);
        
        return services;
    }
    
    public static void UseMmoModule(this WebApplication app)
    {
        app.UseMmoSettingsModule();
        app.UseMmoPlanningModule();
        app.UseMmoWashingModule();
        app.UseMmoDryingModule();
        app.UseMmoReportingModule();
    }
}