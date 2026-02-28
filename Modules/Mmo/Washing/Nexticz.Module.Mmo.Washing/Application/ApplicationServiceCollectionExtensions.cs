using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Nexticz.Lib.Shared.BackgroundServices;
using Nexticz.Module.Mmo.Washing.Application.Batches.Orchestrators.ActivateBatch;
using Nexticz.Module.Mmo.Washing.Application.DoubleClickProtectors;
using Nexticz.Module.Mmo.Washing.Application.FileHandling;
using Nexticz.Module.Mmo.Washing.Application.MessagePublishers;
using Nexticz.Module.Mmo.Washing.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Washing.Application.PipelineBehaviors;
using Nexticz.Module.Mmo.Washing.Application.Printings;
using Nexticz.Module.Mmo.Washing.Application.Simulations;

namespace Nexticz.Module.Mmo.Washing.Application;

internal static class ApplicationServiceCollectionExtensions
{
    
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions));
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
            options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(WashingPostCommandBehavior<,>));
        });
        
        services.AddValidatorsFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions), includeInternalTypes: true);

        services.AddScoped<IWashingNotificationCollector, WashingNotificationCollector>();
        services.AddScoped<IActivateBatchOrchestrator, ActivateBatchOrchestrator>();
        
        services.AddScoped<IDoubleClickProtector, DoubleClickProtector>();
        services.AddScoped<IWashingPrintHandler, WashingPrintHandler>();
        services.AddScoped<IWashingFileHandler, WashingFileHandler>();
        
        services.AddScoped<IMessagePublisher, MessagePublisher>();
        
        services.AddScoped<SimulationValidator>();
        services.AddScoped<SimulationOnDemandBackgroundService>();
        services.AddSingleton<OnDemandBackgroundServiceHost<SimulationOnDemandBackgroundService>>();
        services.AddSingleton<BarcodeGenerator>();
        
        return services;
    }
}