using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Nexticz.Module.Cuzk.Application;
using Nexticz.Module.Cuzk.Infrastructure;
using Nexticz.Module.Cuzk.Presentation;

namespace Nexticz.Module.Cuzk;

public static class ServiceCollectionExtensions
{
    private const string CuzkIsEnabled = nameof(CuzkIsEnabled);
    
    public static IServiceCollection AddCuzkModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        var cuzkIsEnabled = configuration.GetValue<bool>($"FeatureManagement:{CuzkIsEnabled}");
        
        if (!cuzkIsEnabled) 
            return services;
        
        services.AddApplicationLayer();
        services.AddInfrastructureLayer(configuration);

        return services;
    }

    public static async Task UseCuzkModuleAsync(this WebApplication app)
    {
        var featureManager = app.Services.GetRequiredService<IFeatureManager>();
        if (!await featureManager.IsEnabledAsync(CuzkIsEnabled))
            return;
        
        app.MapCuzkMunicipalitiesEndpoints()
            .MapCuzkAddressLocationsEndpoints()
            .MapCuzkAddressLocationSlugsEndpoints()
            .MapCuzkImportsEndpoints()
            .MapCuzkOpenApiContractEndpoints()
            .MapCuzkEconomicSubjectsEndpoints();
    }
}