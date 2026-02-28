using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Sign.SharedKernel.DataAccess;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Infrastructure.BaseRepositories;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure;

internal static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services, IConfiguration configuration)
    {
        var signDbConnectionString = DbConnectionProvider.GetConnectionString(configuration);
        services.AddMarten<IDocumentManagerDocumentStore>("documentmanager", signDbConnectionString, configuration);
        
        services.AddScoped<IDocumentManagerUnitOfWork, DocumentManagerUnitOfWork>();
        services.AddScoped<IDocumentManagerDocumentSessionProvider, DocumentManagerDocumentSessionProvider>();
        services.AddScoped<IDocumentManagerReadOnlyEventStoreRepository, DocumentManagerReadOnlyEventStoreRepository>();
        
        return services;
    }
}