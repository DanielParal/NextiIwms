using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Infrastructure.BaseRepositories;
using Nexticz.Module.Mmo.Reporting.Infrastructure.DriedKits;
using Nexticz.Module.Mmo.Reporting.Infrastructure.LineItemViews;
using Nexticz.Module.Mmo.Reporting.Infrastructure.Shifts;
using Nexticz.Module.Mmo.Reporting.Infrastructure.WashingMachineSpeeds;

namespace Nexticz.Module.Mmo.Reporting.Infrastructure;

internal static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services, IConfiguration configuration)
    {
        var mmoDbConnectionString = DbConnectionProvider.GetConnectionString(configuration);
        services.AddMarten<IReportingDocumentStore>("reporting", mmoDbConnectionString, configuration);
        
        services.AddScoped<IReportingUnitOfWork, ReportingUnitOfWork>();
        services.AddScoped<IReportingDocumentSessionProvider, ReportingDocumentSessionProvider>();
        services.AddScoped<IReportingReadOnlyEventStoreRepository, ReportingReadOnlyEventStoreRepository>();
        
        services.AddScoped<IShiftReadOnlyRepository, ShiftReadOnlyRepository>();
        services.AddScoped<IDriedKitReadOnlyRepository, DriedKitReadOnlyRepository>();
        services.AddScoped<ILineItemReadOnlyRepository, LineItemReadOnlyRepository>();
        services.AddScoped<IWashingMachineSpeedReadOnlyRepository, WashingMachineSpeedReadOnlyRepository>();
        
        return services;
    }
}