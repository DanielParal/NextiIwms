using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nexticz.Lib.Shared.DeploymentInfo;

public static class DeploymentInfoRegistrator
{
    public static IServiceCollection AddDeploymentConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var deploymentSettings = configuration.GetSection(nameof(DeploymentInfoSettings)).Get<DeploymentInfoSettings>()
                           ?? throw new InvalidOperationException("Deployment info settings section is missing.");

        services.AddSingleton(deploymentSettings);
        
        return services;
    }
    
    public static void UseDeploymentConfiguration(this WebApplication app)
    {
        app.UseMiddleware<DeploymentInfoHeaderMiddleware>();
    }
}