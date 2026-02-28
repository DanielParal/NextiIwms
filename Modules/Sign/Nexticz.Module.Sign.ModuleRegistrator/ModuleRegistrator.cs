using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nexticz.Module.Sign.DocumentLoader;
using Nexticz.Module.Sign.DocumentManager;
using Nexticz.Module.Sign.Settings;

namespace Nexticz.Module.Sign.ModuleRegistrator;

public static class ModuleRegistrator
{
    public static IHealthChecksBuilder AddSignHealthChecks(this IHealthChecksBuilder builder, IConfiguration configuration)
    {
        var dbConnectionString = configuration.GetConnectionString("Sign");
        builder.AddNpgSql(
            connectionString: dbConnectionString!,
            name: "Sign db - module database",
            failureStatus: HealthStatus.Unhealthy,
            tags: ["module", "sign", "db", "postgres"]);
        
        return builder;
    }
    
    public static IServiceCollection AddSignModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSignSettingsModule(configuration)
            .AddSignDocumentLoaderModule(configuration)
            .AddSignDocumentManagerModule(configuration);
        
        return services;
    }
    
    public static void UseSignModule(this WebApplication app)
    {
        app.UseSignSettingsModule();
        app.UseSignDocumentLoaderModule();
        app.UseSignDocumentManagerModule();
    }
}