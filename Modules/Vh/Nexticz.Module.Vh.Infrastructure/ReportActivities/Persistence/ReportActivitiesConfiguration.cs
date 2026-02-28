using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.ReportActivities;

namespace Nexticz.Module.Vh.Infrastructure.ReportActivities.Persistence;

public class ReportActivitiesConfiguration : IEntityTypeConfiguration<ReportActivity>
{
    public void Configure(EntityTypeBuilder<ReportActivity> builder)
    {
        builder.ToTable("ReportActivities");
        builder.Property(x => x.WorkerCenterCode).HasMaxLength(100);
        builder.Property(x => x.CenterCode).HasMaxLength(100);
        builder.Property(x => x.WorkerCode).HasMaxLength(100);
        builder.Property(x => x.ActivityCode).HasMaxLength(100);
        builder.Property(x => x.DepositorCode).HasMaxLength(100);
        builder.Property(x => x.DepositorGroupCode).HasMaxLength(100);
        builder.Property(x => x.ActivitySystemType).HasMaxLength(100);
        builder.Property(x => x.Note).HasMaxLength(255);
        builder.Property(x => x.ActivitySource).HasConversion<string>();
        builder.Property(x => x.DurationTime).HasPrecision(6, 2);
        builder.Property(x => x.Coefficient).HasPrecision(6, 2);
        builder.Property(x => x.Score).HasPrecision(6, 2);
        builder.Property(x => x.WorkerShiftPowerPercentage).HasPrecision(6, 2);
    }
}