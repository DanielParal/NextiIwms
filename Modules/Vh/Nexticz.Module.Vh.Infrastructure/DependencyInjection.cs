using DevExtreme.AspNet.Data.Aggregation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Lib.Shared.DataAccess;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.BackgroundWorkers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence.VYKHOD;
using Nexticz.Module.Vh.Infrastructure.ReportActivities.Aggregators;
using StringHelper = Nexticz.Module.Vh.Application.Common.Helpers.StringHelper;

namespace Nexticz.Module.Vh.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddVhInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        CustomAggregators.RegisterAggregator("percentageAverage", typeof(PercentageAverageAggregator<>));

        return services
            .AddPersistence(configuration)
            .AddBackgroundWorkers();
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(DbConnectionFactory.CreateMsSql(configuration),
                x => x.MigrationsHistoryTable(StringHelper.MigrationTableName, StringHelper.MigrationSchema));
        });
        services.AddDbContext<VykhodContext>(
            options =>
            {
                var vykhodConnectionString = configuration.GetConnectionString("KvadosTestConnection");
                options.UseSqlServer(vykhodConnectionString);
            });

        return services;
    }

    private static IServiceCollection AddBackgroundWorkers(this IServiceCollection services)
    {
        services.AddHostedService<CreateLoadedActivitiesFromEventsWorker>();

        return services;
    }
}