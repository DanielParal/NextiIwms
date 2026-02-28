using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Mmo.Reporting.Application;
using Nexticz.Module.Mmo.Reporting.Infrastructure;
using Nexticz.Module.Mmo.Reporting.Presentation;

namespace Nexticz.Module.Mmo.Reporting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMmoReportingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationLayer();
        services.AddInfrastructureLayer(configuration);
        
        return services;
    }
    
    public static void UseMmoReportingModule(this WebApplication app)
    {
        app.MapMmoDriedKitsEndpoints()
            .MapMmoShiftSettingsEndpoints()
            .MapMmoShiftsEndpoints()
            .MapMmoLineItemsEndpoints();
    }
}