using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Infrastructure.BaseRepositories;
using Nexticz.Module.Mmo.Settings.Infrastructure.Constants;
using Nexticz.Module.Mmo.Settings.Infrastructure.Depositors;
using Nexticz.Module.Mmo.Settings.Infrastructure.InactivityTypes;
using Nexticz.Module.Mmo.Settings.Infrastructure.Kits;
using Nexticz.Module.Mmo.Settings.Infrastructure.KitSapDefinitions;
using Nexticz.Module.Mmo.Settings.Infrastructure.KitTypes;
using Nexticz.Module.Mmo.Settings.Infrastructure.Manufactures;
using Nexticz.Module.Mmo.Settings.Infrastructure.PackagingCirculations;
using Nexticz.Module.Mmo.Settings.Infrastructure.Packagings;
using Nexticz.Module.Mmo.Settings.Infrastructure.PackagingTypes;
using Nexticz.Module.Mmo.Settings.Infrastructure.SpecialInformations;
using Nexticz.Module.Mmo.Settings.Infrastructure.Users;
using Nexticz.Module.Mmo.Settings.Infrastructure.WashingMachines;
using Nexticz.Module.Mmo.Settings.Infrastructure.Workers;

namespace Nexticz.Module.Mmo.Settings.Infrastructure;

internal static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services, IConfiguration configuration)
    {
        var mmoDbConnectionString = DbConnectionProvider.GetConnectionString(configuration);
        services.AddMarten<ISettingsDocumentStore>("settings", mmoDbConnectionString, configuration);
        
        services.AddScoped<ISettingsUnitOfWork, SettingsUnitOfWork>();
        services.AddScoped<ISettingsDocumentSessionProvider, SettingsDocumentSessionProvider>();
        services.AddScoped<ISettingsReadOnlyEventStoreRepository, SettingsReadOnlyEventStoreRepository>();
        
        services.AddScoped<IDepositorReadOnlyRepository, DepositorReadOnlyRepository>();
        services.AddScoped<IPackagingTypeReadOnlyRepository, PackagingTypeReadOnlyRepository>();
        services.AddScoped<IPackagingReadOnlyRepository, PackagingReadOnlyRepository>();
        services.AddScoped<IManufactureReadOnlyRepository, ManufactureReadOnlyRepository>();
        services.AddScoped<IPackagingCirculationReadOnlyRepository, PackagingCirculationReadOnlyRepository>();
        services.AddScoped<IKitTypeReadOnlyRepository, KitTypeReadOnlyRepository>();
        services.AddScoped<IKitSapDefinitionReadOnlyRepository, KitSapDefinitionReadOnlyRepository>();
        services.AddScoped<IKitReadOnlyRepository, KitReadOnlyRepository>();
        services.AddScoped<IWashingMachineReadOnlyRepository, WashingMachineReadOnlyRepository>();
        services.AddScoped<IConstantReadOnlyRepository, ConstantReadOnlyRepository>();
        services.AddScoped<IWorkerReadOnlyRepository, WorkerReadOnlyRepository>();
        services.AddScoped<IUserReadOnlyRepository, UserReadOnlyRepository>();
        services.AddScoped<ISpecialInformationReadOnlyRepository, SpecialInformationReadOnlyRepository>();
        services.AddScoped<IInactivityTypeReadOnlyRepository, InactivityTypeReadOnlyRepository>();

        return services;
    }
}