using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.Centers;

namespace Nexticz.Module.Vh.Infrastructure.Centers.Persistence;

public class CentersConfiguration : IEntityTypeConfiguration<Center>
{
    public void Configure(EntityTypeBuilder<Center> builder)
    {
        builder.ToTable("Centers");
        builder.Property(x => x.Name).HasMaxLength(50);
        builder.Property(x => x.Code).HasMaxLength(10);
        builder
            .HasMany(x => x.DepositorsGroups)
            .WithOne(x => x.Center)
            .HasForeignKey(x => x.CenterId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasMany(x => x.Depositors)
            .WithOne(x => x.Center)
            .HasForeignKey(x => x.CenterId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasMany(x => x.Workers)
            .WithOne(x => x.Center)
            .HasForeignKey(x => x.CenterId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasMany(x => x.NonDispensingActivities)
            .WithOne(x => x.Center)
            .HasForeignKey(x => x.CenterId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}