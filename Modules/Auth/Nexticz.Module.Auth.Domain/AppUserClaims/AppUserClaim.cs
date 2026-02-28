using Microsoft.AspNetCore.Identity;
using Nexticz.Module.Auth.Domain.AppUsers;

namespace Nexticz.Module.Auth.Domain.AppUserClaims;

public class AppUserClaim : IdentityUserClaim<Guid>
{
    public AppUser? AppUser { get; set; }
}