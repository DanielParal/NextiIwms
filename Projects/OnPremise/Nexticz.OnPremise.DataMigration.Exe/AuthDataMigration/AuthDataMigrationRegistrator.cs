using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nexticz.OnPremise.DataMigration.Exe.AuthDataMigration;

internal static class AuthDataMigrationRegistrator
{
    public static IServiceCollection AddAuthDataMigration(this IServiceCollection services, IConfiguration configuration)
    {
        var authMsSqlConnectionString = configuration.GetConnectionString("AuthMsSql")
                                 ?? throw new InvalidOperationException("AuthMsSql connection string is missing.");

        var authPostgresConnectionString = configuration.GetConnectionString("AuthPostgres")
                                        ?? throw new InvalidOperationException("AuthPostgres connection string is missing.");

        var settings = new AuthDataMigrationSettings()
        {
            MsSqlConnectionString = authMsSqlConnectionString,
            PostgresConnectionString = authPostgresConnectionString
        };

        services.AddSingleton(settings);
        
        services.AddDbContext<AuthMsSqlContext>(options =>
            options.UseSqlServer(authMsSqlConnectionString));
        
        services.AddDbContext<AuthPostgresContext>(options =>
            options.UseNpgsql(authPostgresConnectionString));

        services.AddHostedService<AuthDataMigrationWorker>();
        
        return services;
    }
}