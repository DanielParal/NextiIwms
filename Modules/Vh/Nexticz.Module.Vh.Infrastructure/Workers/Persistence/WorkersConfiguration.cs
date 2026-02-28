using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.Workers;

namespace Nexticz.Module.Vh.Infrastructure.Workers.Persistence;

public class WorkersConfiguration : IEntityTypeConfiguration<Worker>
{
    public void Configure(EntityTypeBuilder<Worker> builder)
    {
        builder.ToTable("Workers");
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Surname).HasMaxLength(100);
        builder.Property(x => x.CodeWms).HasMaxLength(100);
        builder.Property(x => x.CodeSag).HasMaxLength(100);
        builder.Property(x => x.ActivityAfterCutOffCode).HasMaxLength(100);
        builder
            .HasMany(x => x.LoadingActionsNdas)
            .WithOne(x => x.Worker)
            .HasForeignKey(x => x.WorkerSlug)
            .HasPrincipalKey(x => x.CodeWms)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}