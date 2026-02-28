using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Lang.Presentation.Endpoints;
using Nexticz.Module.Lang.Application;
using Nexticz.Module.Lang.Infrastructure;
using Nexticz.Module.Lang.Infrastructure.Common.Persistence.Extensions;

namespace Nexticz.Module.Lang.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddLangPresentation(this IServiceCollection services)
    {
        return services
            .AddHttpContextAccessor();
    }
    
    public static IServiceCollection AddLangModule(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddLangPresentation()
            .AddLangApplication()
            .AddLangInfrastructure(configuration);
    }
    public static void UseLangModule(this WebApplication app)
    {
         app.MapLangApiEndpoints();
         _ = app.SeedLangDatabaseAsync(app);
    }
}