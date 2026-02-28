using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Lib.Shared.Cors;

public static class CorsRegistrator
{
    public static void UseCorsConfiguration(this WebApplication app, IConfiguration configuration)
    {
        var corsSettings = configuration.GetSection(nameof(CorsSettings)).Get<CorsSettings>()
                               ?? throw new InvalidOperationException("Cors settings section is missing.");

        app.UseCors(policy =>
            policy
                .AllowAnyHeader()
                .WithExposedHeaders(
                    StringHelper.Header.XAuthorizationFailed, 
                    StringHelper.Header.XDeploymentTimestamp, 
                    StringHelper.Header.XTenantTimeZone)
                .AllowAnyMethod()
                .AllowCredentials()
                .WithOrigins(corsSettings.Origins));
    }
}