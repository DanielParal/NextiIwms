using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nexticz.Lib.Shared.Swagger;

public static class SwaggerServiceRegistrator
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(opt =>
        {
            opt.OperationFilter<SwaggerDefaultValues>();
            opt.SupportNonNullableReferenceTypes();
            opt.DescribeAllParametersInCamelCase();
            opt.SchemaFilter<RequiredNotNullableSchemaFilter>();
        });

        return services;
    }
    
    private const string SwaggerEnabled = nameof(SwaggerEnabled);
    public static async Task UseSwaggerConfigurationAsync(this WebApplication app)
    {
        var featureManager = app.Services.GetRequiredService<IFeatureManager>();
        
        if (!await featureManager.IsEnabledAsync(SwaggerEnabled))
            return;

        app.UseSwagger();
        app.UseSwaggerUI(
            options => ConfigureSwaggerOptions.CreateSwaggerJsonFileUrls(app, options));
    }
}