using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Mmo.Settings.Application;
using Nexticz.Module.Mmo.Settings.Infrastructure;
using Nexticz.Module.Mmo.Settings.Presentation;

namespace Nexticz.Module.Mmo.Settings;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMmoSettingsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationLayer();
        services.AddInfrastructureLayer(configuration);
        
        return services;
    }
    
    public static void UseMmoSettingsModule(this WebApplication app)
    {
        app.MapMmoMapOpenApiContractEndpoints()
            .MapMmoKitEndpoints()
            .MapMmoDepositorEndpoints()
            .MapMmoPackagingTypesEndpoints()
            .MapMmoPackagingsEndpoints()
            .MapMmoManufacturesEndpoints()
            .MapMmoPackagingCirculationsEndpoints()
            .MapMmoKitTypesEndpoints()
            .MapMmoMapKitSapDefinitionsEndpoints()
            .MapMmoWashingMachinesEndpoints()
            .MapMmoImportsEndpoints()
            .MapMmoExportsEndpoints()
            .MapMmoHistoryEventsEndpoints()
            .MapMmoSeedsEndpoints()
            .MapMmoMapProjectionEndpoints()
            .MapConstantsEndpoints()
            .MapDeveloperOnlyConstantsEndpoints()
            .MapWorkersEndpoints()
            .MapCompletionPermissionWorkersEndpoints()
            .MapUsersEndpoints()
            .MapMmoSpecialInformationsEndpoints()
            .MapInactivityTypesEndpoints()
            .MapAnyPermissionMmoInactivityTypesEndpoints()
            .MapAnyPermissionMmoKitEndpoints()
            .MapAnyPermissionMmoMapKitSapDefinitionsEndpoints()
            .MapAnyPermissionMmoKitTypesEndpoints()
            .MapAnyPermissionMmoPackagingsEndpoints()
            .MapAnyPermissionMmoManufacturesEndpoints()
            .MapAnyPermissionMmoWashingMachinesEndpoints()
            .MapAnyPermissionMmoDepositorEndpoints()
            .MapAnyPermissionMmoPackagingTypesEndpoints()
            .MapAnyPermissionMmoPackagingCirculationsEndpoints();
    }
}