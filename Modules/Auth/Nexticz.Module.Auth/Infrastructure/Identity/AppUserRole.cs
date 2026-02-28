using Microsoft.AspNetCore.Identity;

namespace Nexticz.Module.Auth.Infrastructure.Identity;

public class AppUserRole : IdentityUserRole<Guid>
{
    public AppUser? AppUser { get; set; }
    public AppRole? AppRole { get; set; }
}