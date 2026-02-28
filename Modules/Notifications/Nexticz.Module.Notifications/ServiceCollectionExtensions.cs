using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Notifications.Infrastructure;
using Nexticz.Module.Notifications.Presentation;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Notifications.Application;
using Nexticz.Module.Notifications.Application.SignalRNotifications;

namespace Nexticz.Module.Notifications;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplicationLayer();
        services.AddInfrastructureLayer(configuration);

        return services;
    }

    public static void UseNotificationsModule(this WebApplication app)
    {
        app.MapNotificationsOpenApiContractEndpoints();
        app.MapNotificationsHubEndpointsExtensions();
    }
}