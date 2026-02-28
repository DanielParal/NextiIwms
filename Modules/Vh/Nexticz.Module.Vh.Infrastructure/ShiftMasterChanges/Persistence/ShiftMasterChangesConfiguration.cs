using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.ShiftMasterChanges;

namespace Nexticz.Module.Vh.Infrastructure.ShiftMasterChanges.Persistence;

public class ShiftMasterChangesConfiguration : IEntityTypeConfiguration<ShiftMasterChange>
{
    public void Configure(EntityTypeBuilder<ShiftMasterChange> builder)
    {
        builder.ToTable("ShiftMasterChanges");
        builder.Property(x => x.User).HasMaxLength(100);
        builder.Property(x => x.CenterCode).HasMaxLength(100);
        builder.Property(x => x.WorkerCode).HasMaxLength(100);
        builder.Property(x => x.ShiftMasterActivityType).HasConversion<string>();
        builder.OwnsMany(x => x.WorkerShiftChanges, wsch =>
        {
            wsch.ToTable("WorkerShiftChanges");
            wsch.Property(a => a.WorkerCode).HasMaxLength(100);
            wsch.Property(a => a.WorkerCenterCode).HasMaxLength(100);
            wsch.Property(x => x.ContextChangeType).HasConversion<string>();
        });
        builder.OwnsMany(x => x.WorkerShiftActivityChanges, wsach =>
        {
            wsach.ToTable("WorkerShiftActivityChanges");
            wsach.Property(x => x.ContextChangeType).HasConversion<string>();
            wsach.Property(a => a.WorkerCode).HasMaxLength(100);
            wsach.Property(a => a.CenterCode).HasMaxLength(100);
            wsach.Property(a => a.ActivityCode).HasMaxLength(100);
            wsach.Property(a => a.ActivityType).HasMaxLength(100);
            wsach.Property(a => a.ActivitySource).HasMaxLength(100);
            wsach.Property(a => a.Note).HasMaxLength(100);
            wsach.Property(x => x.ActivityType).HasConversion<string>();
            wsach.Property(x => x.ActivitySource).HasConversion<string>();
            wsach.Property(x => x.Unit).HasMaxLength(100);
            wsach.Property(x => x.Coefficient).HasPrecision(6, 2);
            wsach.Property(x => x.DurationMinutes).HasPrecision(6, 2);
            wsach.Property(x => x.ActivityCountOrDurationMinutes).HasPrecision(6, 2);
            wsach.Property(x => x.Score).HasPrecision(6, 2);
        });
    }
}