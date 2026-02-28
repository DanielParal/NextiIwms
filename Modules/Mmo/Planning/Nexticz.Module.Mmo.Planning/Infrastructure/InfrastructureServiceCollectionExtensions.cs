using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;

using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Infrastructure.BaseRepositories;
using Nexticz.Module.Mmo.Planning.Infrastructure.WashingMachines;

namespace Nexticz.Module.Mmo.Planning.Infrastructure;

internal static class InfrastructureServiceCollectionExtensions
{
     public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services, IConfiguration configuration)
    {
        var mmoDbConnectionString = DbConnectionProvider.GetConnectionString(configuration);
        services.AddMarten<IPlanningDocumentStore>("planning", mmoDbConnectionString, configuration);
        
        services.AddScoped<IPlanningUnitOfWork, PlanningUnitOfWork>();
        services.AddScoped<IPlanningDocumentSessionProvider, PlanningDocumentSessionProvider>();
        services.AddScoped<IPlanningReadOnlyEventStoreRepository, PlanningReadOnlyEventStoreRepository>();

        services.AddScoped<IWashingMachineReadOnlyRepository, WashingMachineReadOnlyRepository>();
        
        return services;
    }
}