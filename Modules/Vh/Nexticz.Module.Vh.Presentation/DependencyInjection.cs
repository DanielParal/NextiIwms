using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Vh.Application;
using Nexticz.Module.Vh.Infrastructure;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence.Extensions;
using Nexticz.Module.Vh.Presentation.Endpoints;

namespace Nexticz.Module.Vh.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddVhPresentation(this IServiceCollection services)
    {
        return services
            .AddHttpContextAccessor();
    }
    
    public static IServiceCollection AddVhModule(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddVhPresentation()
            .AddVhApplication()
            .AddVhInfrastructure(configuration);
    }

    public static void UseVhModule(this WebApplication app)
    {
        app.MapVhApiEndpoints();
        _ = app.SeedVhDatabaseAsync(app);
    }
}