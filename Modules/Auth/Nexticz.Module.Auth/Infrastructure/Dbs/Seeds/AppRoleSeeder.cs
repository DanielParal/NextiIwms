using Microsoft.AspNetCore.Identity;
using Nexticz.Module.Auth.Domain;
using Nexticz.Module.Auth.Domain.UserAggregate;
using Nexticz.Module.Auth.Infrastructure.Identity;

namespace Nexticz.Module.Auth.Infrastructure.Dbs.Seeds;

public class AppRoleSeeder(RoleManager<AppRole> roleManager)
{
    public async Task SeedAsync()
    {
        var roles = new List<AppRole>
        {
            new() { Name = nameof(RoleType.Developer) },
            new() { Name = nameof(RoleType.SysAdmin) },
            new() { Name = nameof(RoleType.Anonymous) }
        };

        foreach (var appRole in roles)
        {
            if (await roleManager.RoleExistsAsync(appRole.Name!)) 
                continue;
            
            await roleManager.CreateAsync(appRole);
        }
    }
}