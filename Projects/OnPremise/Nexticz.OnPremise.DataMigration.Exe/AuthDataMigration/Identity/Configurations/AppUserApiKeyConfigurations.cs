using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nexticz.OnPremise.DataMigration.Exe.AuthDataMigration.Identity;

public class AppUserApiKeyConfigurations : IEntityTypeConfiguration<AppUserApiKey>
{
    public void Configure(EntityTypeBuilder<AppUserApiKey> builder)
    {
        builder.ToTable("AppUserApiKeys");
        builder.Property(x => x.Description).HasMaxLength(255);
        builder.Property(x => x.Value).HasMaxLength(255);
    }
}