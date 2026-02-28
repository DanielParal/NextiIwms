using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Portal.Application;
using Nexticz.Module.Portal.Infrastructure;
using Nexticz.Module.Portal.Presentation;

namespace Nexticz.Module.Portal;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPortalModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplicationLayer();
        services.AddInfrastructureLayer(configuration);

        return services;
    }

    public static void UsePortalModule(this WebApplication app)
    {
        app.MapPortalModulesEndpoints()
            .MapPortalModulesAnyPermissionEndpoints()
            .MapPortalSeedsEndpoints();
    }
}