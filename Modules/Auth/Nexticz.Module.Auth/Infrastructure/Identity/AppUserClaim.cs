using Microsoft.AspNetCore.Identity;

namespace Nexticz.Module.Auth.Infrastructure.Identity;

public class AppUserClaim : IdentityUserClaim<Guid>
{
    public AppUser? AppUser { get; set; }
}