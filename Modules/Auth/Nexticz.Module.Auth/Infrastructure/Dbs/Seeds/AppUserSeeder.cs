using Microsoft.AspNetCore.Identity;
using Nexticz.Module.Auth.Domain;
using Nexticz.Module.Auth.Domain.UserAggregate;
using Nexticz.Module.Auth.Infrastructure.Identity;

namespace Nexticz.Module.Auth.Infrastructure.Dbs.Seeds;

public class AppUserSeeder(UserManager<AppUser> userManager)
{
    public async Task SeedAsync()
    {
        if (userManager.Users.Any(user => user.UserName == "podpora@nexti.cz"))
            return;
        
        var developerUser = new AppUser
        (
            "podpora@nexti.cz",
            "podpora@nexti.cz",
            "+420774793940",
            null,
            null,
            "Next internet, s.r.o."
        );
        
        var userLoginInfo = new UserLoginInfo(nameof(AuthenticationMethodType.UsenamePassword),
            Guid.NewGuid().ToString(), nameof(AuthenticationMethodType.UsenamePassword));
        
        await userManager.CreateAsync(developerUser);
        await userManager.AddToRoleAsync(developerUser, nameof(RoleType.Developer));
        await userManager.AddLoginAsync(developerUser, userLoginInfo);
    }
}