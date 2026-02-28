using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Portal.Application.Interfaces;
using Nexticz.Module.Portal.Infrastructure.BaseRepositories;

namespace Nexticz.Module.Portal.Infrastructure;

internal static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services, IConfiguration configuration)
    {
        var postgresConnectionString = configuration.GetConnectionString("Portal") ??
                                       throw new InvalidOperationException("Portal PostgresDb connection string not found.");
        
        services.AddMarten<IPortalDocumentStore>("portal", postgresConnectionString, configuration);
        
        services.AddScoped<IPortalUnitOfWork, PortalUnitOfWork>();
        services.AddScoped<IPortalDocumentSessionProvider, PortalDocumentSessionProvider>();
        services.AddScoped<IPortalReadOnlyEventStoreRepository, PortalReadOnlyEventStoreRepository>();
        
        return services;
    }
}