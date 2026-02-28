using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Lang.Application.Common.Interfaces;
using Nexticz.Lib.Shared.DataAccess;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Lang.Infrastructure.Common.Persistence;
using StringHelper = Nexticz.Module.Lang.Application.Common.Helpers.StringHelper;

namespace Nexticz.Module.Lang.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLangInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddPersistance(configuration);
    }

    private static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(DbConnectionFactory.CreateMsSql(configuration),
                x => x.MigrationsHistoryTable(StringHelper.MigrationTableName, StringHelper.MigrationSchema));
        });

        return services;
    }
}