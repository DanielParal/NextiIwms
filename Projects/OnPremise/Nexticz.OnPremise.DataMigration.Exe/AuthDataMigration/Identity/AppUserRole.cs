using Microsoft.AspNetCore.Identity;

namespace Nexticz.OnPremise.DataMigration.Exe.AuthDataMigration.Identity;

public class AppUserRole : IdentityUserRole<Guid>
{
    public AppUser? AppUser { get; set; }
    public AppRole? AppRole { get; set; }
}