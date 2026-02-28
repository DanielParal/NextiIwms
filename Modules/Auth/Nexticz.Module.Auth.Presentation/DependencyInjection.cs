using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Auth.Application;
using Nexticz.Module.Auth.Infrastructure;
using Nexticz.Module.Auth.Infrastructure.Common.Persistence.Extensions;
using Nexticz.Module.Auth.Presentation.Endpoints;

namespace Nexticz.Module.Auth.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthPresentation(this IServiceCollection services)
    {
        return services
            .AddHttpContextAccessor();
        ;
    }

    public static IServiceCollection AddAuthModule(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddAuthPresentation()
            .AddAuthApplication()
            .AddAuthInfrastructure(configuration);
    }

    public static void UseAuthModule(this WebApplication app)
    {
        app.MapAuthApiEndpoints();
        _ = app.SeedAuthDatabaseAsync(app);
    }
}