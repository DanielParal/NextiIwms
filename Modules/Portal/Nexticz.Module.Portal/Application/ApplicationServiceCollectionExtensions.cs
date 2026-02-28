using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Portal.Application.Grouping;
using Nexticz.Module.Portal.Application.NotificationCollectors;
using Nexticz.Module.Portal.Application.PipelineBehaviors;
using Nexticz.Module.Portal.Application.Seeds.Orchestrators;

namespace Nexticz.Module.Portal.Application;

internal static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions));
            options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PortalPostCommandBehavior<,>));
        });
        
        services.AddScoped<IPortalGroupedResultHandler, PortalGroupedResultHandler>();
        services.AddScoped<INotificationCollector, NotificationCollector>();
        
        services.AddScoped<ISeedOrchestrator, SeedOrchestrator>();
        
        return services;
    }
}