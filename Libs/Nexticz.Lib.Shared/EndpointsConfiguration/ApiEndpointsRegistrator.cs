using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Nexticz.Lib.Shared.EndpointsConfiguration;

public static class ApiEndpointsRegistrator
{
    public static IServiceCollection AddEndpointsConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddControllersWithViews()
            .AddJsonOptions(opt => { opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
        services.ConfigureHttpJsonOptions(opt =>
        {
            opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        return services;
    }
}

