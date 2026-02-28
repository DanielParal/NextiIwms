using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Sign.DocumentLoader.Application;
using Nexticz.Module.Sign.DocumentLoader.Infrastructure;

namespace Nexticz.Module.Sign.DocumentLoader;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSignDocumentLoaderModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationLayer();
        services.AddInfrastructureLayer(configuration);
        
        return services;
    }
    
    public static void UseSignDocumentLoaderModule(this WebApplication app)
    {
    }
}