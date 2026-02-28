using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Sign.DocumentManager.Application;
using Nexticz.Module.Sign.DocumentManager.Infrastructure;
using Nexticz.Module.Sign.DocumentManager.Presentation;

namespace Nexticz.Module.Sign.DocumentManager;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSignDocumentManagerModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationLayer();
        services.AddInfrastructureLayer(configuration);
        
        return services;
    }
    
    public static void UseSignDocumentManagerModule(this WebApplication app)
    {
        app.MapSignUnsignedLoadingDocumentsEndpoints()
            .MapSignSharedSigningDevicesEndpoints()
            .MapSignSigningDevicesForUserEndpoints()
            .MapSignSigningDevicesForDeviceEndpoints()
            .MapSignSignedLoadingDocumentsEndpoints()
            .MapSignDeleteLoadingDocumentsEndpoints()
            .MapSignMapLoadingDocumentsEndpoints()
            .MapSignProjectionEndpoints()
            .MapSignEmailsEndpoints();
    }
}