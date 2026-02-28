using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.LoadingActionsNdas;

namespace Nexticz.Module.Vh.Infrastructure.LoadingActionsNdas.Persistence;

public class LoadingActionsNdasConfiguration : IEntityTypeConfiguration<LoadingActionsNda>
{
    public void Configure(EntityTypeBuilder<LoadingActionsNda> builder)
    {
        builder.ToTable("LoadingActionsNdas");
        builder.Property(x => x.Note).HasMaxLength(255);
        builder.Property(x => x.WorkerSlug).HasMaxLength(100);
        builder.Property(x => x.NonDispensingActivitySlug).HasMaxLength(100);
    }
}