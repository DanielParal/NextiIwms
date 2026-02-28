using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Auth.Domain.AppRoles;
using Nexticz.Module.Auth.Domain.AppUsers;

namespace Nexticz.Module.Auth.Infrastructure.Common.Persistence.Initialization.Seeds;

public class AppUserSeed
{
    private readonly IServiceProvider _serviceProvider;

    public AppUserSeed(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task RunSeed()
    {
        var userManager = _serviceProvider.GetRequiredService<UserManager<AppUser>>();

        var userLoginInfo = new UserLoginInfo(nameof(AuthenticationMethodType.UsenamePassword),
            Guid.NewGuid().ToString(), nameof(AuthenticationMethodType.UsenamePassword));

        if (!userManager.Users.Any(user => user.UserName == "podpora@nexti.cz"))
        {
            var developerUser = new AppUser
            (
                "podpora@nexti.cz",
                "podpora@nexti.cz",
                "+420774793940",
                null,
                null,
                "Next internet, s.r.o."
            );
            await userManager.CreateAsync(developerUser);
            await userManager.AddToRoleAsync(developerUser, AppRoleType.Developer.ToString());
            await userManager.AddLoginAsync(developerUser, userLoginInfo);
        }
    }
}