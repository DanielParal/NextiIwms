using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Infrastructure.BaseRepositories;
using Nexticz.Module.Mmo.Washing.Infrastructure.Batches;
using Nexticz.Module.Mmo.Washing.Infrastructure.KitCounters;
using Nexticz.Module.Mmo.Washing.Infrastructure.LastEnteredWorkerOnLines;
using Nexticz.Module.Mmo.Washing.Infrastructure.WashingMachineSoses;
using Nexticz.Module.Mmo.Washing.Infrastructure.WashingMachineSpeeds;

namespace Nexticz.Module.Mmo.Washing.Infrastructure;

internal static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services, IConfiguration configuration)
    {
        var mmoDbConnectionString = DbConnectionProvider.GetConnectionString(configuration);
        services.AddMarten<IWashingDocumentStore>("washing", mmoDbConnectionString, configuration);
        
        services.AddScoped<IWashingUnitOfWork, WashingUnitOfWork>();
        services.AddScoped<IWashingDocumentSessionProvider, WashingDocumentSessionProvider>();
        services.AddScoped<IWashingReadOnlyEventStoreRepository, WashingReadOnlyEventStoreRepository>();
        services.AddScoped<IBatchReadOnlyRepository, BatchReadOnlyRepository>();
        services.AddScoped<ILastEnteredWorkerOnLineRepository, LastEnteredWorkerOnLineRepository>();
        services.AddScoped<IWashingMachineSpeedReadOnlyRepository, WashingMachineSpeedReadOnlyRepository>();
        services.AddScoped<IWashingMachineSosReadOnlyRepository, WashingMachineSosReadOnlyRepository>();
        
        services.AddSingleton<IGlobalKitsCounter, GlobalKitsCounter>();
        
        services.AddHostedService<GlobalKitsCounterInitializer>();
        services.AddHostedService<WashingMachineSpeedSyncWorker>();
        
        return services;
    }
}