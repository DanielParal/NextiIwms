using Microsoft.Extensions.Configuration;

namespace Nexticz.Lib.Shared.Swagger;

internal static class SwaggerSettingsFactory
{
    public static SwaggerSettings Create(IConfiguration configuration)
    {
        return configuration.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>()  
               ?? throw new InvalidOperationException("Swagger Settings not found.");
    }   
}