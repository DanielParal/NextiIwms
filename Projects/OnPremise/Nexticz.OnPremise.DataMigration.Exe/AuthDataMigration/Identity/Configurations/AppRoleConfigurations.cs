using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nexticz.OnPremise.DataMigration.Exe.AuthDataMigration.Identity.Configurations;

public class AppRoleConfigurations : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.ToTable(name: "AppRoles");
        builder
            .HasMany(x => x.AppUserRoles)
            .WithOne(x => x.AppRole)
            .HasForeignKey(x => x.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}