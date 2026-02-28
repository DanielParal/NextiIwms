using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Mmo.Planning.Application;
using Nexticz.Module.Mmo.Planning.Infrastructure;
using Nexticz.Module.Mmo.Planning.Presentation;

namespace Nexticz.Module.Mmo.Planning;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMmoPlanningModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationLayer();
        services.AddInfrastructureLayer(configuration);
        
        return services;
    }
    
    public static void UseMmoPlanningModule(this WebApplication app)
    {
        app.MapMmoWashingMachinesEndpoints();
    }
}