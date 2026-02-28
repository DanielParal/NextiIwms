using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Notifications.Application.NotificationCollectors;
using Nexticz.Module.Notifications.Application.PipelineBehaviors;
using Nexticz.Module.Notifications.Application.SignalRNotifications;

namespace Nexticz.Module.Notifications.Application;

internal static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddSignalR();
        
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions));
            options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(NotificationPostCommandBehavior<,>));
        });
        
        services.AddScoped<INotificationCollector, NotificationCollector>();
        services.AddScoped<ISignalRNotifier, SignalRNotifier>();
        
        return services;
    }
}