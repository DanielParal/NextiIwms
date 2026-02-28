using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nexticz.Lib.Shared.FileHandling.Assets;

public static class AssetsRegistrator
{
    public static IServiceCollection AddAssetsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var assetsSettings = configuration.GetSection(nameof(AssetsSettings)).Get<AssetsSettings>()
                             ?? throw new InvalidOperationException($"{nameof(AssetsSettings)} settings section is missing.");
        
        services.AddSingleton(assetsSettings);

        return services;
    }
}