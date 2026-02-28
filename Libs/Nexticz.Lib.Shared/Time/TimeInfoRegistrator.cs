
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nexticz.Lib.Shared.Time;

public static class TimeInfoRegistrator
{
    public static IServiceCollection AddTimeInfo(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IClock, SystemClock>();
        services.Configure<TimeZoneSettings>(
            configuration.GetSection(nameof(TimeZoneSettings))
        );

        return services;
    }
    
    public static IApplicationBuilder UseTimeInfo(this IApplicationBuilder app)
    {
        app.UseMiddleware<TenantTimeZoneMiddleware>();
        return app;
    }
}