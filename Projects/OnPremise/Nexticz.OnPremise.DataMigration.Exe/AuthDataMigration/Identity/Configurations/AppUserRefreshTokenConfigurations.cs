using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nexticz.OnPremise.DataMigration.Exe.AuthDataMigration.Identity.Configurations;

public class AppUserRefreshTokenConfigurations : IEntityTypeConfiguration<AppUserRefreshToken>
{
    public void Configure(EntityTypeBuilder<AppUserRefreshToken> builder)
    {
        RelationalEntityTypeBuilderExtensions.ToTable((EntityTypeBuilder)builder, name: "AppUserRefreshTokens");
        builder.Property(x => x.UserDeviceInfo).HasMaxLength(255);
        builder.Property(x => x.Value).HasMaxLength(255);
    }
}