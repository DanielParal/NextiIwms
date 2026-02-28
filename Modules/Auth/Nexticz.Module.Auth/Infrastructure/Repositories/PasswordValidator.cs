using Microsoft.AspNetCore.Identity;
using Nexticz.Module.Auth.Application.Interfaces;
using Nexticz.Module.Auth.Infrastructure.Identity;

namespace Nexticz.Module.Auth.Infrastructure.Repositories;

internal class PasswordValidator(UserManager<AppUser> userManager) : IPasswordValidator
{
    public async Task<bool> IsPasswordValidAsync(string username, string password, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(username);
        if (user is null)
            return false;
        
        return await userManager.CheckPasswordAsync(user, password);
    }
}