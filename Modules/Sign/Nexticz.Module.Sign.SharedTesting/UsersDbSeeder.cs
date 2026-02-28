using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Sign.SharedKernel.Security;
using Nexticz.Module.Auth.Domain.AppRoles;
using Nexticz.Module.Auth.Domain.AppUserApiKeys;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Module.Auth.Infrastructure.Common.Persistence;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Sign.SharedTesting;

public class UsersDbSeeder
{
    public const string XApiKeyDeveloper = "Kj9pX7mQ2vL8nRw+YtBcFiOZ5e/HUwxk3qrPJD4z0Na1gsMVo2Epl9hd6BvTR8cuWPYQK7t4NxAlKRQjm5s+fA==";
    private const string DeveloperUserName = "developer@example.com";
    private const string DeveloperPassword = "TestDeveloperPassword123!";

    public static async Task SeedAuthUsersAsync(IServiceScope scope)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        
        await CreateRolesAsync(roleManager);
        await CreateDeveloperUserWithApiKeyAsync(userManager, dbContext);
    }
    
    private static async Task CreateRolesAsync(RoleManager<AppRole> roleManager)
    {
        var roles = new List<AppRole>
        {
            new() { Name = nameof(AuthorizationHelper.Role.Developer) },
            new() { Name = nameof(Role.SignMember) }
        };

        foreach (var appRole in roles)
        {
            if (await roleManager.RoleExistsAsync(appRole.Name!)) 
                continue;
            await roleManager.CreateAsync(appRole);
        }
    }

    private static async Task CreateDeveloperUserWithApiKeyAsync(UserManager<AppUser> userManager, DataContext dbContext)
    {
        var developerUser = new AppUser
        {
            UserName = "Developer",
            Email = DeveloperUserName,
            Company = "Test Company",
            Firstname = "John",
            Lastname = "Doe",
            Registered = DateTime.UtcNow,
            LastActivity = DateTime.UtcNow,
            EmailConfirmed = true,
            PhoneNumber = "+1234567890",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            AccessFailedCount = 0
        };

        await userManager.CreateAsync(developerUser, DeveloperPassword);
        await userManager.AddToRoleAsync(developerUser, nameof(AuthorizationHelper.Role.Developer));
        await userManager.AddToRoleAsync(developerUser, nameof(Role.SignMember));
        
        var adminApiKey = new AppUserApiKey
        {
            Id = Guid.NewGuid(),
            Value = XApiKeyDeveloper,
            Created = DateTime.UtcNow,
            LastActivity = DateTime.UtcNow,
            Expiration = DateTime.UtcNow.AddYears(1),
            UserId = developerUser.Id,
            Description = "Test API Key for Integration Tests"
        };
        
        dbContext.AppUserApiKeys.Add(adminApiKey);
        await dbContext.SaveChangesAsync();
    }
}