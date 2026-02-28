using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Drying.Application.Interfaces;
using Nexticz.Module.Mmo.Drying.Infrastructure.BaseRepositories;
using Nexticz.Module.Mmo.Drying.Infrastructure.Kits;

namespace Nexticz.Module.Mmo.Drying.Infrastructure;

internal static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services, IConfiguration configuration)
    {
        var mmoDbConnectionString = DbConnectionProvider.GetConnectionString(configuration);
        services.AddMarten<IDryingDocumentStore>("drying", mmoDbConnectionString, configuration);
        
        services.AddScoped<IDryingUnitOfWork, DryingUnitOfWork>();
        services.AddScoped<IDryingDocumentSessionProvider, DryingDocumentSessionProvider>();
        services.AddScoped<IDryingReadOnlyEventStoreRepository, DryingReadOnlyEventStoreRepository>();
        
        services.AddScoped<IKitReadOnlyRepository, KitReadOnlyRepository>();
        
        return services;
    }
}