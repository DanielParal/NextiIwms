using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Auth.Application;
using Nexticz.Module.Auth.Infrastructure;

namespace Nexticz.Module.Auth;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthModuleV2(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplicationLayer();
        services.AddInfrastructureLayer(configuration);

        return services;
    }

    public static async Task UseAuthModuleV2Async(this WebApplication app)
    {
        await app.UseInfrastructureLayerAsync();
    }
}