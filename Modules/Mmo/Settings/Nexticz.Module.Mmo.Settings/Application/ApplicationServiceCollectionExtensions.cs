using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Settings.Application.FileHandling;
using Nexticz.Module.Mmo.Settings.Application.Grouping;
using Nexticz.Module.Mmo.Settings.Application.ImportsExports.Exports.ExportHandlers;
using Nexticz.Module.Mmo.Settings.Application.ImportsExports.Imports.ImportHandlers;
using Nexticz.Module.Mmo.Settings.Application.Kits;
using Nexticz.Module.Mmo.Settings.Application.Kits.ImportExport;
using Nexticz.Module.Mmo.Settings.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Settings.Application.Packagings.ImportExport;
using Nexticz.Module.Mmo.Settings.Application.PipelineBehaviors;
using Nexticz.Module.Mmo.Settings.Application.Seeds.SeedHandlers;
using Nexticz.Module.Mmo.Settings.Application.Users.Orchestrators;
using Nexticz.Module.Mmo.Settings.Domain.ExportEntity;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;

namespace Nexticz.Module.Mmo.Settings.Application;

internal static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions));
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
            options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(SettingsPostCommandBehavior<,>));
        });
        
        services.AddValidatorsFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions), includeInternalTypes: true);

        services.AddScoped<ISettingsGroupedResultHandler, SettingsGroupedResultHandler>();
        services.AddScoped<ISettingsNotificationCollector, SettingsNotificationCollector>();
        
        services.AddScoped<ISettingsImportHandlerSelector, SettingsImportHandlerSelector>();
        services.AddScoped<ISettingsImportHandler<ImportType>, PackagingImportXlsxHandler>();
        services.AddScoped<ISettingsImportHandler<ImportType>, KitImportXlsxHandler>();
        
        services.AddScoped<ISettingsExportHandlerSelector, SettingsExportHandlerSelector>();
        services.AddScoped<ISettingsExportHandler<ExportType>, PackagingExportXlsxHandler>();
        services.AddScoped<ISettingsExportHandler<ExportType>, KitExportXlsxHandler>();
        
        services.AddScoped<ISeedRunner, DepositorSeedHandler>();
        services.AddScoped<ISeedRunner, KitTypeSeedHandler>();
        services.AddScoped<ISeedRunner, ManufactureSeedHandler>();
        services.AddScoped<ISeedRunner, PackagingCirculationSeedHandler>();
        services.AddScoped<ISeedRunner, PackagingTypeSeedHandler>();
        services.AddScoped<ISeedRunner, WashingMachineSeedHandler>();
        services.AddScoped<ISeedRunner, ConstantSeedHandler>();
        services.AddScoped<ISeedRunner, KitSapDefinitionSeedHandler>();
        
        services.AddScoped<ISettingsFileHandler, SettingsFileHandler>();
        
        services.AddScoped<IUserChangedOrchestrator, UserChangedOrchestrator>();

        services.AddScoped<KitResponseFactory>();
        
        return services;
    }
}