using Microsoft.AspNetCore.Identity;
using Nexticz.Module.Auth.Domain.AppUserRoles;

namespace Nexticz.Module.Auth.Domain.AppRoles;

public class AppRole : IdentityRole<Guid>
{
    public ICollection<AppUserRole>? AppUserRoles { get; set; }
}