using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Auth.Application.Common.Interfaces;
using Nexticz.Module.Auth.Domain.AppRoles;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Lib.Shared.BaseUrls;
using Nexticz.Lib.Shared.DataAccess;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Auth.Infrastructure.Authentication.TokenGenerator;
using Nexticz.Module.Auth.Infrastructure.Common.Interfaces;
using Nexticz.Module.Auth.Infrastructure.Common.Persistence;
using Nexticz.Module.Auth.Infrastructure.Common.Services;
using Refit;
using StringHelper = Nexticz.Module.Auth.Application.Common.Helpers.StringHelper;

namespace Nexticz.Module.Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>()
            .AddScoped<IAppUserService, AppUserService>()
            .AddPersistence(configuration)
            .AddThirdPartyApis(configuration)
            .AddIdentity();
    }

    private static IServiceCollection AddThirdPartyApis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRefitClient<ILangApiService>();
        services.AddRefitClient<IHealtService>();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrlSettings = BaseUrlSettingsFactory.Create(configuration);
        
        services.AddRefitClient<ILangApiService>()
            .ConfigureHttpClient(c =>
                c.BaseAddress = new Uri(baseUrlSettings.Api + "/api/lang"));
        services.AddRefitClient<IHealtService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrlSettings.Api));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(DbConnectionFactory.CreateMsSql(configuration),
                x =>
                {
                    x.MigrationsHistoryTable(StringHelper.MigrationTableName, StringHelper.MigrationSchema);
                    x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                }
            );
        });

        return services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentityCore<AppUser>()
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 4;
            options.SignIn.RequireConfirmedAccount = false;
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            options.Lockout.MaxFailedAccessAttempts = 10;
        });

        return services;
    }
}