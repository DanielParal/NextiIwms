using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Mmo.SharedKernel.Security;
using Nexticz.Module.Auth.Infrastructure.Common.Persistence;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Auth.Domain.AppRoles;
using Nexticz.Module.Auth.Domain.AppUserApiKeys;
using Nexticz.Module.Auth.Domain.AppUsers;

namespace Nexticz.Module.Mmo.SharedTesting;

public class UsersDbSeeder
{
    public const string XApiKeyDeveloper = "Kj9pX7mQ2vL8nRw+YtBcFiOZ5e/HUwxk3qrPJD4z0Na1gsMVo2Epl9hd6BvTR8cuWPYQK7t4NxAlKRQjm5s+fA==";
    private const string DeveloperUserName = "developer@example.com";
    private const string DeveloperPassword = "TestDeveloperPassword123!";

    public const string XApiKeyMember = "MouzCN8I4uFjjGQ+CmLfLiNX1o/GSzyk5wreUEp3yYi3vsRPo1Ask2gd5EvPR6buTOVQK6p9PhYlNRQokq8t+g==";
    private const string MemberUserName = "member@example.com";
    private const string MemberPassword = "TestMemberPassword123!";
    
    public const string XApiKeyForkLiftLoaderMember = "Bx7nY4qW8uN3vS6+CnMeKiOY3p/IUxyk7wsfUEr5yAj5wtTPo3Cuk4he7FxRS8cvVQXRL8r1QzCnPSRqln0v+i==";
    private const string ForkLiftLoaderMemberUserName = "ForkLiftLoadermember@example.com";
    private const string ForkLiftLoaderMemberPassword = "TestForkLiftLoaderMemberPassword123!";

    public static async Task SeedAuthUsersAsync(IServiceScope scope)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        
        await CreateRolesAsync(roleManager);
        await CreateDeveloperUserWithApiKeyAsync(userManager, dbContext);
        await CreateMemberUserWithApiKeyAsync(userManager, dbContext);
        await CreateForkLiftLoaderMemberUserWithApiKeyAsync(userManager, dbContext);
    }
    
    private static async Task CreateRolesAsync(RoleManager<AppRole> roleManager)
    {
        var roles = new List<AppRole>
        {
            new() { Name = nameof(AuthorizationHelper.Role.Developer) },
            new() { Name = nameof(Role.MmoMember) }
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
        await userManager.AddToRoleAsync(developerUser, nameof(Role.MmoMember));
        
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
    
    private static async Task CreateMemberUserWithApiKeyAsync(UserManager<AppUser> userManager, DataContext dbContext)
    {
        var memberUser = new AppUser
        {
            UserName = "Member",
            Email = MemberUserName,
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
        
        await userManager.CreateAsync(memberUser, MemberPassword);
        await userManager.AddToRoleAsync(memberUser, nameof(Role.MmoMember));
        
        var memberApiKey = new AppUserApiKey
        {
            Id = Guid.NewGuid(),
            Value = XApiKeyMember,
            Created = DateTime.UtcNow,
            LastActivity = DateTime.UtcNow,
            Expiration = DateTime.UtcNow.AddYears(1),
            UserId = memberUser.Id,
            Description = "Test API Key for Integration Tests"
        };

        dbContext.AppUserApiKeys.Add(memberApiKey);
        await dbContext.SaveChangesAsync();
    }
    
    private static async Task CreateForkLiftLoaderMemberUserWithApiKeyAsync(UserManager<AppUser> userManager, DataContext dbContext)
    {
        var memberUser = new AppUser
        {
            UserName = "ForkliftMember",
            Email = ForkLiftLoaderMemberUserName,
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
        
        await userManager.CreateAsync(memberUser, ForkLiftLoaderMemberPassword);
        await userManager.AddToRoleAsync(memberUser, nameof(Role.MmoMember));
        
        var mmoClaims = new List<Claim>
        {
            new(StringHelper.Claim.Type.MagicPermissions, nameof(Permission.MmoManagePlanning))
        };
        await userManager.AddClaimsAsync(memberUser, mmoClaims);
        
        var memberApiKey = new AppUserApiKey
        {
            Id = Guid.NewGuid(),
            Value = XApiKeyForkLiftLoaderMember,
            Created = DateTime.UtcNow,
            LastActivity = DateTime.UtcNow,
            Expiration = DateTime.UtcNow.AddYears(1),
            UserId = memberUser.Id,
            Description = "Test API Key for Integration Tests"
        };

        dbContext.AppUserApiKeys.Add(memberApiKey);
        await dbContext.SaveChangesAsync();
    }
}