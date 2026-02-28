using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Auth.Domain.AppRoles;
using Nexticz.Lib.Shared.Helpers;
using StringHelper = Nexticz.Lib.Shared.Helpers.StringHelper;

namespace Nexticz.Module.Auth.Infrastructure.Common.Persistence.Initialization.Seeds;

public class AppRoleSeed
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public AppRoleSeed(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public async Task RunSeed()
    {


        var roleManager = _serviceProvider.GetRequiredService<RoleManager<AppRole>>();
        var roles = new List<AppRole>
        {
            new() { Name = nameof(AppRoleType.Developer) },
            new() { Name = nameof(AppRoleType.SysAdmin) },
            new() { Name = nameof(AppRoleType.Anonymous) }
        };

        foreach (var appRole in roles)
        {
            if (await roleManager.RoleExistsAsync(appRole.Name!)) 
                continue;
            
            await roleManager.CreateAsync(appRole);
        }
    }
}