using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Sign.DocumentLoader.Application.FileHandling;
using Nexticz.Module.Sign.DocumentLoader.Application.FileProcessors.Orchestrator;
using Nexticz.Module.Sign.DocumentLoader.Application.NotificationCollectors;
using Nexticz.Module.Sign.DocumentLoader.Application.PipelineBehaviors;

namespace Nexticz.Module.Sign.DocumentLoader.Application;

internal static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions));
            options.AddOpenBehavior(typeof(DocumentLoaderValidationBehavior<,>));
            options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(DocumentLoaderPostCommandBehavior<,>));
        });
        
        services.AddValidatorsFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions), includeInternalTypes: true);
        services.AddScoped<IDocumentLoaderNotificationCollector, DocumentLoaderNotificationCollector>();

        services.AddScoped<IFileProcessorOrchestrator, FileProcessorOrchestrator>();
        services.AddSingleton<IDocumentLoaderFileHandler, DocumentLoaderFileHandler>();
        
        return services;
    }
}