using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.Centers;
using Nexticz.Module.Vh.Domain.VhUsers;

namespace Nexticz.Module.Vh.Infrastructure.VhUsers.Persistence;

public class VhUsersConfiguration : IEntityTypeConfiguration<VhUser>
{
    public void Configure(EntityTypeBuilder<VhUser> builder)
    {
        builder.ToTable("VhUsers");
        builder.Property(x => x.Username).HasMaxLength(255);
        builder
            .HasMany(u => u.Centers)
            .WithMany(c => c.VhUsers)
            .UsingEntity<Dictionary<string, object>>(
                "VhUsersCenters",
                j => j
                    .HasOne<Center>()
                    .WithMany()
                    .HasForeignKey("CenterId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<VhUser>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
            );
    }
}