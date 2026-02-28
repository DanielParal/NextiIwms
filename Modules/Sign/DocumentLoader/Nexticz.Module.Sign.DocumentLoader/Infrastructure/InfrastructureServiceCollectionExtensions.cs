using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Sign.SharedKernel.DataAccess;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.DocumentLoader.Application.Interfaces;
using Nexticz.Module.Sign.DocumentLoader.Infrastructure.BaseRepositories;
using Nexticz.Module.Sign.DocumentLoader.Infrastructure.Ftps;

namespace Nexticz.Module.Sign.DocumentLoader.Infrastructure;

internal static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services, IConfiguration configuration)
    {
        var signDbConnectionString = DbConnectionProvider.GetConnectionString(configuration);
        services.AddMarten<IDocumentLoaderDocumentStore>("documentloader", signDbConnectionString, configuration);
        
        services.AddScoped<IDocumentLoaderUnitOfWork, DocumentLoaderUnitOfWork>();
        services.AddScoped<IDocumentLoaderDocumentSessionProvider, DocumentLoaderDocumentSessionProvider>();
        services.AddScoped<IDocumentLoaderReadOnlyEventStoreRepository, DocumentLoaderReadOnlyEventStoreRepository>();

        services.AddHostedService<FileProcessorWorker>();

        services.Configure<TransferDocumentsFtpSettings>(configuration.GetSection("SignTransferDocumentsFtpServer"));
        
        services.AddSingleton<IDocumentLoaderFtpHandler, DocumentLoaderFtpHandler>();
        
        return services;
    }
}