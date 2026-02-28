using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Auth.Infrastructure.Identity;

namespace Nexticz.Module.Auth.Infrastructure.Dbs.TableConfigurations;

public class AppUserRefreshTokenConfigurations : IEntityTypeConfiguration<AppUserRefreshToken>
{
    public void Configure(EntityTypeBuilder<AppUserRefreshToken> builder)
    {
        builder.ToTable(name: "AppUserRefreshTokens");
        builder.Property(x => x.UserDeviceInfo).HasMaxLength(255);
        builder.Property(x => x.Value).HasMaxLength(255);
    }
}