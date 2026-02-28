using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Sign.Settings.Application;
using Nexticz.Module.Sign.Settings.Infrastructure;
using Nexticz.Module.Sign.Settings.Presentation;

namespace Nexticz.Module.Sign.Settings;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSignSettingsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationLayer();
        services.AddInfrastructureLayer(configuration);
        
        return services;
    }
    
    public static void UseSignSettingsModule(this WebApplication app)
    {
        app.MapSignDepositorGroupsEndpoints()
            .MapSignDeliveryMethodsEndpoints()
            .MapSignPartnersEndpoints()
            .MapSignLocationsEndpoints()
            .MapSignPrintersEndpoints()
            .MapSignSigningDevicesEndpoints()
            .MapSignMapDepositorsEndpoints()
            .MapSignMapReceiversEndpoints()
            .MapSignMapUsersEndpoints()
            .MapSignHistoryEventsEndpoints()
            .MapSignEmailConfigurationsEndpoints()
            .MapSignMapOpenApiContractEndpoints()
            .MapSignImportsEndpoints()
            .MapSignExportEndpoints()
            .MapSignEmailTemplatesEndpoints()
            .MapSignDeveloperOnlyEmailTemplatesEndpoints()
            .MapSignDocumentTemplatesEndpoints()
            .MapSignDeveloperOnlyDocumentTemplatesEndpoints()
            .MapSignSeedsEndpoints()
            .MapSignProjectionEndpoints()
            .MapSignDeveloperOnlyConstantsEndpoints()
            .MapSignConstantsEndpoints();
    }
}