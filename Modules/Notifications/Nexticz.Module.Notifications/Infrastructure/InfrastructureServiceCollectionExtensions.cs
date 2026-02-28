using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Notifications.Application.Interfaces;
using Nexticz.Module.Notifications.Infrastructure.BaseRepositories;

namespace Nexticz.Module.Notifications.Infrastructure;

internal static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services, IConfiguration configuration)
    {
        var postgresConnectionString = configuration.GetConnectionString("Notifications") ??
                                       throw new InvalidOperationException("Notifications PostgresDb connection string not found.");
        
        services.AddMarten<INotificationDocumentStore>("notifications", postgresConnectionString, configuration);
        
        services.AddScoped<INotificationUnitOfWork, NotificationUnitOfWork>();
        services.AddScoped<INotificationDocumentSessionProvider, NotificationDocumentSessionProvider>();
        services.AddScoped<INotificationReadOnlyEventStoreRepository, NotificationReadOnlyEventStoreRepository>();
        
        return services;
    }
}