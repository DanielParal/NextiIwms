using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Sign.Settings.Application.DeliveryMethods.ImportExport;
using Nexticz.Module.Sign.Settings.Application.DepositorGroups.ImportExport;
using Nexticz.Module.Sign.Settings.Application.Depositors.ImportExport;
using Nexticz.Module.Sign.Settings.Application.FileHandling;
using Nexticz.Module.Sign.Settings.Application.Grouping;
using Nexticz.Module.Sign.Settings.Application.ImportsExports.Exports.ExportHandlers;
using Nexticz.Module.Sign.Settings.Application.ImportsExports.Imports.ImportHandlers;
using Nexticz.Module.Sign.Settings.Application.NotificationCollectors;
using Nexticz.Module.Sign.Settings.Application.Partners.ImportExport;
using Nexticz.Module.Sign.Settings.Application.PipelineBehaviors;
using Nexticz.Module.Sign.Settings.Application.Receivers.ImportExport;
using Nexticz.Module.Sign.Settings.Application.Seeds.Orchestrators;
using Nexticz.Module.Sign.Settings.Application.Users.Orchestrators;
using Nexticz.Module.Sign.Settings.Domain.ExportAggregate;
using Nexticz.Module.Sign.Settings.Domain.ImportAggregate;

namespace Nexticz.Module.Sign.Settings.Application;

internal static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions));
            options.AddOpenBehavior(typeof(SettingsValidationBehavior<,>));
            options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(SettingsPostCommandBehavior<,>));
        });
        
        services.AddValidatorsFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions), includeInternalTypes: true);

        services.AddScoped<ISettingsGroupedResultHandler, SettingsGroupedResultHandler>();
        services.AddScoped<ISettingsNotificationCollector, SettingsNotificationCollector>();

        services.AddScoped<ISettingsFileHandler, SettingsFileHandler>();
        services.AddScoped<IUserChangedOrchestrator, UserChangedOrchestrator>();
        
        services.AddScoped<ISettingsImportHandlerSelector, SettingsImportHandlerSelector>();
        services.AddScoped<ISettingsImportHandler<ImportType>, PartnerImportXlsxHandler>();
        services.AddScoped<ISettingsImportHandler<ImportType>, ReceiverImportXlsxHandler>();
        services.AddScoped<ISettingsImportHandler<ImportType>, DeliveryMethodImportXlsxHandler>();
        services.AddScoped<ISettingsImportHandler<ImportType>, DepositorGroupImportXlsxHandler>();
        services.AddScoped<ISettingsImportHandler<ImportType>, DepositorImportXlsxHandler>();
        
        services.AddScoped<ISettingsExportHandlerSelector, SettingsExportHandlerSelector>();
        services.AddScoped<ISettingsExportHandler<ExportType>, PartnerExportXlsxHandler>();
        services.AddScoped<ISettingsExportHandler<ExportType>, ReceiverExportXlsxHandler>();
        services.AddScoped<ISettingsExportHandler<ExportType>, DeliveryMethodExportXlsxHandler>();
        services.AddScoped<ISettingsExportHandler<ExportType>, DepositorGroupExportXlsxHandler>();
        services.AddScoped<ISettingsExportHandler<ExportType>, DepositorExportXlsxHandler>();
        
        services.AddScoped<ISeedOrchestrator, SeedOrchestrator>();
        
        return services;
    }
}