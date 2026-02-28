using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Sign.SharedKernel.DataAccess;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Infrastructure.BaseRepositories;

namespace Nexticz.Module.Sign.Settings.Infrastructure;

internal static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services, IConfiguration configuration)
    {
        var signDbConnectionString = DbConnectionProvider.GetConnectionString(configuration);
        services.AddMarten<ISettingsDocumentStore>("settings", signDbConnectionString, configuration);
        
        services.AddScoped<ISettingsUnitOfWork, SettingsUnitOfWork>();
        services.AddScoped<ISettingsDocumentSessionProvider, SettingsDocumentSessionProvider>();
        services.AddScoped<ISettingsReadOnlyEventStoreRepository, SettingsReadOnlyEventStoreRepository>();
        
        return services;
    }
}