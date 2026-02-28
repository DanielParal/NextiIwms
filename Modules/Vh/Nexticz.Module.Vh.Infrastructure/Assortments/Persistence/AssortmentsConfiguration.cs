using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.Assortments;

namespace Nexticz.Module.Vh.Infrastructure.Assortments.Persistence;

public class AssortmentsConfiguration : IEntityTypeConfiguration<Assortment>
{
    public void Configure(EntityTypeBuilder<Assortment> builder)
    {
        builder.ToTable("Assortments");
        builder.Property(x => x.Name).HasMaxLength(255);
        builder.Property(x => x.Code).HasMaxLength(100);
        builder.Property(x => x.ReceiptCoefficient).HasPrecision(6, 2);
        builder.Property(x => x.DispatchCoefficient).HasPrecision(6, 2);
        builder.Property(x => x.PackagingCoefficient).HasPrecision(6, 2);
    }
}