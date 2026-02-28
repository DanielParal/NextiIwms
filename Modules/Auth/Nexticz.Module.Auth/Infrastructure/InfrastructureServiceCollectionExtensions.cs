using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Auth.Infrastructure.Identity;
using Nexticz.Module.Auth.Application.Interfaces;
using Nexticz.Module.Auth.Infrastructure.Dbs;
using Nexticz.Module.Auth.Infrastructure.Dbs.Seeds;
using Nexticz.Module.Auth.Infrastructure.MasstransitPublishers;
using Nexticz.Module.Auth.Infrastructure.Repositories;

namespace Nexticz.Module.Auth.Infrastructure;

internal static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services, IConfiguration configuration)
    {
        var postgresConnectionString = configuration.GetConnectionString("Auth") ??
                                       throw new InvalidOperationException("Auth PostgresDb connection string not found.");
        
        services.AddScoped<IAuthPublisher, AuthPublisher>();
        services.AddScoped<IUserReadOnlyRepository, UserReadOnlyRepository>();
        services.AddScoped<IUserWriteRepository, UserWriteRepository>();
        services.AddScoped<IAuthUnitOfWork, AuthUnitOfWork>();
        services.AddScoped<IPasswordValidator, PasswordValidator>();
        services.AddScoped<IApiKeyWriteRepository, ApiKeyWriteRepository>();
        services.AddScoped<IApiKeyReadOnlyRepository, ApiKeyReadOnlyRepository>();
        
        services.AddDbContext<AuthDataContext>(options =>
        {
            options.UseNpgsql(postgresConnectionString,
                x =>
                {
                    x.MigrationsHistoryTable(AuthDataContext.MigrationTableName);
                    x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                }
            );
        });
        
        services.AddScoped<AppRoleSeeder>()
            .AddScoped<AppUserSeeder>()
            .AddScoped<TranslationsSeeder>()
            .AddScoped<IDatabaseInitializer, DatabaseInitializer>()
            .AddIdentitySetup();
        
        return services;
    }

    public static async Task UseInfrastructureLayerAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var dbInitializer = services.GetRequiredService<IDatabaseInitializer>();
        await dbInitializer.InitializeAsync(CancellationToken.None);
    }
}