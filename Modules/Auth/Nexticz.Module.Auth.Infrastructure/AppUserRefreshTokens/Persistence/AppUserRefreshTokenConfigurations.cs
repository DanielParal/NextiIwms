using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Auth.Domain.AppUserRefreshTokens;

namespace Nexticz.Module.Auth.Infrastructure.AppUserRefreshTokens.Persistence;

public class AppUserRefreshTokenConfigurations : IEntityTypeConfiguration<AppUserRefreshToken>
{
    public void Configure(EntityTypeBuilder<AppUserRefreshToken> builder)
    {
        builder.ToTable(name: "AppUserRefreshTokens");
        builder.Property(x => x.UserDeviceInfo).HasMaxLength(255);
        builder.Property(x => x.Value).HasMaxLength(255);
    }
}