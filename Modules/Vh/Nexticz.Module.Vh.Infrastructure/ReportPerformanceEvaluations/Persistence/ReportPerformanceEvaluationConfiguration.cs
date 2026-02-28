using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.ReportPerformanceEvaluations;

namespace Nexticz.Module.Vh.Infrastructure.ReportPerformanceEvaluations.Persistence;

public class ReportPerformanceEvaluationConfiguration : IEntityTypeConfiguration<ReportPerformanceEvaluation>
{
    public void Configure(EntityTypeBuilder<ReportPerformanceEvaluation> builder)
    {
        builder.ToTable("ReportPerformanceEvaluations");
        builder.Property(x => x.CenterCode).HasMaxLength(100);
        builder.Property(x => x.WorkerCode).HasMaxLength(100);
        builder.Property(x => x.WorkerName).HasMaxLength(100);
        builder.Property(x => x.DurationTime).HasPrecision(6, 2);
        builder.Property(x => x.Score).HasPrecision(6, 2);
        builder.Property(x => x.ScoreMyStock).HasPrecision(6, 2);
        builder.Property(x => x.ScoreIwms).HasPrecision(6, 2);
        builder.Property(x => x.ScoreSag).HasPrecision(6, 2);
        builder.Property(x => x.ScoreNonProductive).HasPrecision(6, 2);
        builder.Property(x => x.Salary).HasPrecision(6, 2);
        builder.Property(x => x.Zone).HasPrecision(6, 2);
    }
}