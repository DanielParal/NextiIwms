using Microsoft.AspNetCore.Identity;

namespace Nexticz.OnPremise.DataMigration.Exe.AuthDataMigration.Identity;

public class AppUserClaim : IdentityUserClaim<Guid>
{
    public AppUser? AppUser { get; set; }
}