using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Lib.Shared.Versioning;

public static class ApiVersioningRegistrator
{
    public static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
    {
        services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.ApiVersionReader = new HeaderApiVersionReader(StringHelper.Header.XApiVersion);
        }).AddApiExplorer();

        return services;
    }
}