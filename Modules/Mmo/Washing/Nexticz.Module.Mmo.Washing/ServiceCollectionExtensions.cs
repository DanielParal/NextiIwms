using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Mmo.Washing.Application;
using Nexticz.Module.Mmo.Washing.Infrastructure;
using Nexticz.Module.Mmo.Washing.Presentation;

namespace Nexticz.Module.Mmo.Washing;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMmoWashingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationLayer();
        services.AddInfrastructureLayer(configuration);
        
        return services;
    }
    
    public static void UseMmoWashingModule(this WebApplication app)
    {
        app
            .MapMmoBatchesEndpoints()
            .MapMmoMapLastEnteredWorkerOnLineEndpoints()
            .MapMmoWashingStateEndpoints()
            .MapMmoWashingMachineSosEndpoints()
            .MapMmoSimulationEndpoints();
    }
}