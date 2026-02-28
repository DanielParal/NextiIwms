using Microsoft.AspNetCore.Identity;

namespace Nexticz.OnPremise.DataMigration.Exe.AuthDataMigration.Identity;

public class AppRole : IdentityRole<Guid>
{
    public ICollection<AppUserRole>? AppUserRoles { get; set; }
}