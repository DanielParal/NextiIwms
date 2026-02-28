using Microsoft.AspNetCore.Identity;
using Nexticz.Module.Auth.Domain.AppRoles;
using Nexticz.Module.Auth.Domain.AppUsers;

namespace Nexticz.Module.Auth.Domain.AppUserRoles;

public class AppUserRole : IdentityUserRole<Guid>
{
    public AppUser? AppUser { get; set; }
    public AppRole? AppRole { get; set; }
}