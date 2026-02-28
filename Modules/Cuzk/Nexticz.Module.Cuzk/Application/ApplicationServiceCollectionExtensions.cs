using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Cuzk.Application.ElasticSearch;
using Nexticz.Module.Cuzk.Application.AddressLocations.ImportExport;
using Nexticz.Module.Cuzk.Application.FileHandling;
using Nexticz.Module.Cuzk.Application.Grouping;
using Nexticz.Module.Cuzk.Application.ImportsExports.Imports.ImportHandlers;
using Nexticz.Module.Cuzk.Application.ImportsExports.Imports.Orchestrators;
using Nexticz.Module.Cuzk.Application.MasstransitPublishers;
using Nexticz.Module.Cuzk.Application.Municipalities.ImportExport;
using Nexticz.Module.Cuzk.Application.NotificationCollectors;
using Nexticz.Module.Cuzk.Application.PipelineBehaviors;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Application;

internal static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions));
            options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CuzkPostCommandBehavior<,>));
        });
        
        services.AddScoped<ICuzkGroupedResultHandler, CuzkGroupedResultHandler>();
        services.AddScoped<INotificationCollector, NotificationCollector>();
        
        services.AddScoped<ICuzkFileHandler, CuzkFileHandler>();
        
        services.AddScoped<ICuzkImportHandlerSelector, CuzkImportHandlerSelector>();
        services.AddScoped<ICuzkImportHandler<ImportType>, MunicipalityImportCsvHandler>();
        services.AddScoped<ICuzkImportHandler<ImportType>, AddressLocationImportCsvHandler>();
        
        services.AddScoped<IImportProcessorOrchestrator, ImportProcessorOrchestrator>();
        services.AddScoped<IMasstransitPublisher, MasstransitPublisher>();
        
        return services;
    }
}