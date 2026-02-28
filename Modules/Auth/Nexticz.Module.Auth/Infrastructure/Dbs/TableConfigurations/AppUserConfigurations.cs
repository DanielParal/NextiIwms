using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Auth.Infrastructure.Identity;

namespace Nexticz.Module.Auth.Infrastructure.Dbs.TableConfigurations;

public class AppUserConfigurations : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable(name: "AppUsers");
        builder.Property(x => x.Company).HasMaxLength(100);
        builder.Property(x => x.Firstname).HasMaxLength(50);
        builder.Property(x => x.Lastname).HasMaxLength(50);
        builder
            .HasMany(x => x.AppUserRoles)
            .WithOne(x => x.AppUser)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasMany(x => x.AppUserClaims)
            .WithOne(x => x.AppUser)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasMany(x => x.AppUserRefreshTokens)
            .WithOne(x => x.AppUser)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasMany(x => x.AppUserApiKeys)
            .WithOne(x => x.AppUser)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}