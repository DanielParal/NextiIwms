using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.NonDispensingActivities;

namespace Nexticz.Module.Vh.Infrastructure.NonDispensingActivities.Persistence;

public class NonDispensingActivitiesConfiguration : IEntityTypeConfiguration<NonDispensingActivity>
{
    public void Configure(EntityTypeBuilder<NonDispensingActivity> builder)
    {
        builder.ToTable(name: "NonDispensingActivities");
        builder.Property(x => x.ActivityIdentifier).HasMaxLength(100);
        builder.Property(x => x.Name).HasMaxLength(255);
        builder.Property(x => x.Note).HasMaxLength(100);
        builder.Property(x => x.Unit).HasMaxLength(100);
        builder.Property(x => x.Coefficient).HasPrecision(6, 2);
        builder
            .HasMany(x => x.LoadingActionsNdas)
            .WithOne(x => x.NonDispensingActivity)
            .HasForeignKey(x => x.NonDispensingActivitySlug)
            .HasPrincipalKey(x => x.ActivityIdentifier)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}