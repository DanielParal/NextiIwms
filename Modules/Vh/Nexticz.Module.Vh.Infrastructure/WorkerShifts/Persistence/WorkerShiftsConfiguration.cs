using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.ReportPerformanceEvaluations;
using Nexticz.Module.Vh.Domain.WorkerShifts;

namespace Nexticz.Module.Vh.Infrastructure.WorkerShifts.Persistence;

public class WorkerShiftsConfiguration : IEntityTypeConfiguration<WorkerShift>
{
    public void Configure(EntityTypeBuilder<WorkerShift> builder)
    {
        builder.ToTable("WorkerShifts");
        builder.Property(x => x.WorkerCode).HasMaxLength(100);
        builder.Property(x => x.WorkerCenterCode).HasMaxLength(100);
        builder.Property(x => x.ActivityAfterCutOffCode).HasMaxLength(100);
        builder.Property(x => x.ActivityAfterCutOffName).HasMaxLength(100);
        builder.OwnsMany(x => x.Activities, activity =>
        {
            activity.ToTable("WorkerShiftActivities");
            activity.Property(a => a.WorkerCode).HasMaxLength(100);
            activity.Property(a => a.CenterCode).HasMaxLength(100);
            activity.Property(a => a.ActivityCode).HasMaxLength(100);
            activity.Property(a => a.DepositorCode).HasMaxLength(100);
            activity.Property(a => a.DepositorGroupCode).HasMaxLength(100);
            activity.Property(a => a.ActivitySystemType).HasMaxLength(100);
            activity.Property(a => a.ActivityType).HasMaxLength(100);
            activity.Property(a => a.ActivitySource).HasMaxLength(100);
            activity.Property(a => a.Note).HasMaxLength(100);
            activity.Property(a => a.ActivityName).HasMaxLength(100);
            activity.Property(x => x.ActivityType).HasConversion<string>();
            activity.Property(x => x.ActivitySource).HasConversion<string>();
            activity.Property(x => x.Unit).HasMaxLength(100);
            activity.Property(x => x.Coefficient).HasPrecision(6, 2);
            activity.Property(x => x.DurationMinutes).HasPrecision(10, 2);
            activity.Property(x => x.ActivityCountOrDurationMinutes).HasPrecision(10, 2);
            activity.Property(x => x.Score).HasPrecision(10, 2);

        });
        builder
            .HasMany(x => x.ReportActivities)
            .WithOne(x => x.WorkerShift)
            .HasForeignKey(x => x.WorkerShiftId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(x => x.ReportPerformanceEvaluation)
            .WithOne(x => x.WorkerShift)
            .HasForeignKey<ReportPerformanceEvaluation>(x => x.WorkerShiftId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}