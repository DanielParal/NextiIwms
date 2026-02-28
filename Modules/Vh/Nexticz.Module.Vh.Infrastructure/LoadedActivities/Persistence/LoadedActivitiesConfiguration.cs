using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.LoadedActivities;

namespace Nexticz.Module.Vh.Infrastructure.LoadedActivities.Persistence;

public class LoadedActivitiesConfiguration : IEntityTypeConfiguration<LoadedActivity>
{
    public void Configure(EntityTypeBuilder<LoadedActivity> builder)
    {
        builder.ToTable("LoadedActivities");
        builder.Property(x => x.WorkerCode).HasMaxLength(100);
        builder.Property(x => x.CenterCode).HasMaxLength(100);
        builder.Property(x => x.ActivityCode).HasMaxLength(100);
        builder.Property(x => x.DepositorCode).HasMaxLength(100);
        builder.Property(x => x.DepositorGroupCode).HasMaxLength(100);
        builder.Property(x => x.ActivitySystemType).HasMaxLength(100);
        builder.Property(x => x.Note).HasMaxLength(255);
        builder.Property(x => x.ActivityName).HasMaxLength(255);
        builder.Property(x => x.PDoklad).HasMaxLength(255);
        builder.Property(x => x.SortKod).HasMaxLength(255);
        builder.Property(x => x.LicenceKod).HasMaxLength(255);
        builder.Property(x => x.ActivityType).HasConversion<string>();
        builder.Property(x => x.ActivitySource).HasConversion<string>();
        builder.Property(x => x.ActivityState).HasConversion<string>();
        builder.Property(x => x.Unit).HasMaxLength(100);
        builder.Property(x => x.Coefficient).HasPrecision(6, 2);
        builder.HasIndex(x => x.ActivitySource)
            .HasDatabaseName("IX_LoadedActivities_ActivitySource");

        builder.HasIndex(x => x.Created)
            .HasDatabaseName("IX_LoadedActivities_Created");

        builder.HasIndex(x => new { x.ActivitySource, x.Created })
            .HasDatabaseName("IX_LoadedActivities_ActivitySource_Created");
    }
}