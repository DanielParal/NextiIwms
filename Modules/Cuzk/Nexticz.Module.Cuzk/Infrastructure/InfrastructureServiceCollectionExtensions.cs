using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Cuzk.Application.ElasticSearch;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Infrastructure.BaseRepositories;
using Nexticz.Module.Cuzk.Infrastructure.EconomicSubjects;
using Nexticz.Module.Cuzk.Infrastructure.ElasticSearch;
using Nexticz.Module.Cuzk.Infrastructure.Imports;

namespace Nexticz.Module.Cuzk.Infrastructure;

internal static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services, IConfiguration configuration)
    {
        var postgresConnectionString = configuration.GetConnectionString("Cuzk") ??
                                       throw new InvalidOperationException("Cuzk PostgresDb connection string not found.");
        
        services.AddMarten<ICuzkDocumentStore>("cuzk", postgresConnectionString, configuration);
        
        services.AddScoped<ICuzkUnitOfWork, CuzkUnitOfWork>();
        services.AddScoped<ICuzkDocumentSessionProvider, CuzkDocumentSessionProvider>();
        services.AddScoped<ICuzkReadOnlyEventStoreRepository, CuzkReadOnlyEventStoreRepository>();

        services.AddScoped<IEconomicSubjectsClient, EconomicSubjectsClient>();
        
        var govEconomicSubjectOptions = configuration
            .GetSection(GovEconomicSubjectOptions.Key)
            .Get<GovEconomicSubjectOptions>() ?? throw new InvalidOperationException("Gov Economic Subject options not found.");
        
        services.AddHttpClient(EconomicSubjectsClient.HttpClientGovEconomicSubjects,
            client => { client.BaseAddress = new Uri(govEconomicSubjectOptions.BaseUrl); });
        
        services.AddHostedService<ImportProcessorWorker>();
        
        services.AddElasticsearch(configuration);
        services.AddScoped<IAddressLocationSearch, AddressLocationSearch>();
        
        return services;
    }
}