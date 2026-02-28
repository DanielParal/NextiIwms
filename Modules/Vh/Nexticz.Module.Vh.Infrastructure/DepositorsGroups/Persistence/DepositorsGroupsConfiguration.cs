using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.DepositorsGroups;

namespace Nexticz.Module.Vh.Infrastructure.DepositorsGroups.Persistence;

public class DepositorsGroupsConfiguration : IEntityTypeConfiguration<DepositorsGroup>
{
    public void Configure(EntityTypeBuilder<DepositorsGroup> builder)
    {
        builder.ToTable("DepositorsGroups");
        builder.Property(x => x.Name).HasMaxLength(50);
        builder.Property(x => x.Code).HasMaxLength(50);
        builder
            .HasMany(x => x.Depositors)
            .WithOne(x => x.DepositorsGroup)
            .HasForeignKey(x => x.DepositorsGroupId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasMany(x => x.SystemActivities)
            .WithOne(x => x.DepositorsGroup)
            .HasForeignKey(x => x.DepositorGroupId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}