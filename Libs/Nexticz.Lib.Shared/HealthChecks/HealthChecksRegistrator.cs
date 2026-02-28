using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Nexticz.Lib.Shared.BaseUrls;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Lib.Shared.HealthChecks;

public static class HealthChecksRegistrator
{
    private const string HealthChecksEnabled = nameof(HealthChecksEnabled);
    
    public static IServiceCollection AddHealthChecksUiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>($"FeatureManagement:{HealthChecksEnabled}"))
            return services;
        
        var baseUrlSettings = BaseUrlSettingsFactory.Create(configuration);
        var apiUrl = baseUrlSettings.Api;
        
        var healthChecksSettings = configuration.GetSection(nameof(HealthChecksSettings)).Get<HealthChecksSettings>()
                           ?? throw new InvalidOperationException($"{nameof(HealthChecksSettings)} settings section is missing.");
        
        services.AddHealthChecksUI(options =>
            {
                options.SetEvaluationTimeInSeconds(180); 
                options.SetMinimumSecondsBetweenFailureNotifications(180);
                
                foreach (var group in healthChecksSettings.Groups)
                {
                    options.AddHealthCheckEndpoint(group.UiName, $"{apiUrl}/{group.Url}");
                }
            })
            .AddInMemoryStorage();

        return services;
    }
    
    
    public static async Task UseHealthChecksAsync(this WebApplication app, IConfiguration configuration)
    {
        var featureManager = app.Services.GetRequiredService<IFeatureManager>();
        
        if (!await featureManager.IsEnabledAsync(HealthChecksEnabled))
            return;
        
        var healthChecksSettings = configuration.GetSection(nameof(HealthChecksSettings)).Get<HealthChecksSettings>()
                                   ?? throw new InvalidOperationException($"{nameof(HealthChecksSettings)} settings section is missing.");

        foreach (var group in healthChecksSettings.Groups)
        {
            app.MapHealthChecks(group.Url, new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains(group.GroupTagName),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        }

        app.MapHealthChecksUI(options =>
        {
            options.UIPath = "/health-ui";
        });
    }
}