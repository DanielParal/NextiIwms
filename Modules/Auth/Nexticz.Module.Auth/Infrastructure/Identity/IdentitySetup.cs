using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Auth.Infrastructure.Dbs;

namespace Nexticz.Module.Auth.Infrastructure.Identity;

internal static class IdentitySetup
{
    public static IServiceCollection AddIdentitySetup(this IServiceCollection services)
    {
        services.AddIdentityCore<AppUser>()
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<AuthDataContext>()
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