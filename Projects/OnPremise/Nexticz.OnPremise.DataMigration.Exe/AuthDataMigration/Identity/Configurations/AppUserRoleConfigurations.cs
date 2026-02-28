using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nexticz.OnPremise.DataMigration.Exe.AuthDataMigration.Identity.Configurations;

public class AppUserRoleConfigurations : IEntityTypeConfiguration<AppUserRole>
{
    public void Configure(EntityTypeBuilder<AppUserRole> builder)
    {
        RelationalEntityTypeBuilderExtensions.ToTable((EntityTypeBuilder)builder, name: "AppUserRoles");
    }
}