using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Sign.DocumentManager.Application.EmailRenderers;
using Nexticz.Module.Sign.DocumentManager.Application.Emails.Orchestrators;
using Nexticz.Module.Sign.DocumentManager.Application.FileHandling;
using Nexticz.Module.Sign.DocumentManager.Application.Grouping;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Orchestrators;
using Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers;
using Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.EmailPublishers;
using Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.PrintingPublishers;
using Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.SignalRNotifications;
using Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.SigningPublishers;
using Nexticz.Module.Sign.DocumentManager.Application.NotificationCollectors;
using Nexticz.Module.Sign.DocumentManager.Application.PipelineBehaviors;
using Nexticz.Module.Sign.DocumentManager.Application.PrintingHandling;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Orchestrators;

namespace Nexticz.Module.Sign.DocumentManager.Application;

internal static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions));
            options.AddOpenBehavior(typeof(DocumentManagerValidationBehavior<,>));
            options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(DocumentManagerPostCommandBehavior<,>));
        });
        
        services.AddValidatorsFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions), includeInternalTypes: true);
        services.AddScoped<IDocumentManagerNotificationCollector, DocumentManagerNotificationCollector>();
        services.AddScoped<IDocumentManagerGroupedResultHandler, DocumentManagerGroupedResultHandler>();
        
        services.AddSingleton<IDocumentManagerFileHandler, DocumentManagerFileHandler>();
        services.AddScoped<ISignDocumentsOrchestrator, SignDocumentsOrchestrator>();
        services.AddScoped<IDocumentManagerPrintHandler, DocumentManagerPrintHandler>();
        
        services.AddScoped<IDocumentManagerPublisher, DocumentManagerPublisher>();
        services.AddScoped<IDocumentMovementNotifier, DocumentMovementNotifier>();
        services.AddScoped<IDocumentPrintingNotifier, DocumentPrintingNotifier>();
        services.AddScoped<IDocumentSigningNotifier, DocumentSigningNotifier>();
        services.AddScoped<IDocumentManagerEmailPublisher, DocumentManagerEmailPublisher>();
        services.AddScoped<IDocumentManagerPrintingPublisher, DocumentManagerPrintingPublisher>();
        services.AddScoped<IDocumentManagerSigningPublisher, DocumentManagerSigningPublisher>();
        
        services.AddScoped<ILoadingDocumentEmailRenderer, LoadingDocumentEmailRenderer>();
        services.AddScoped<IDeliveryDocumentEmailRenderer, DeliveryDocumentEmailRenderer>();
        
        services.AddScoped<ISendEmailsOrchestrator, SendEmailsOrchestrator>();
        services.AddScoped<IDownloadSignedDocumentsOrchestrator, DownloadSignedDocumentsOrchestrator>();
        services.AddScoped<IDeviceCommunicationOrchestrator, DeviceCommunicationOrchestrator>();
        services.AddScoped<IDeleteDocumentsOrchestrator, DeleteDocumentsOrchestrator>();
        services.AddScoped<IDocumentsPrintingOrchestrator, DocumentsPrintingOrchestrator>();
        
        return services;
    }
}