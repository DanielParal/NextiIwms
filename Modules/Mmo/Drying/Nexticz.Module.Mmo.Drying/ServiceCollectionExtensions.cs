using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Mmo.Drying.Application;
using Nexticz.Module.Mmo.Drying.Infrastructure;
using Nexticz.Module.Mmo.Drying.Presentation;

namespace Nexticz.Module.Mmo.Drying;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMmoDryingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationLayer();
        services.AddInfrastructureLayer(configuration);
        
        return services;
    }
    
    public static void UseMmoDryingModule(this WebApplication app)
    {
        app.MapMmoKitsEndpoints()
            .MapMmoDashboardKitsEndpoints();
    }
}