using Microsoft.AspNetCore.Identity;

namespace Nexticz.Module.Auth.Infrastructure.Identity;

public class AppRole : IdentityRole<Guid>
{
    public ICollection<AppUserRole>? AppUserRoles { get; set; }
}