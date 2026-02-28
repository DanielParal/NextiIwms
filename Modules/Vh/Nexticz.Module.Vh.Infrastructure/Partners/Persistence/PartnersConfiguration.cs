using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.Partners;

namespace Nexticz.Module.Vh.Infrastructure.Partners.Persistence;

public class PartnersConfiguration : IEntityTypeConfiguration<Partner>
{
    public void Configure(EntityTypeBuilder<Partner> builder)
    {
        builder.ToTable("Partners");
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Code).HasMaxLength(100);
        builder.Property(x => x.Note).HasMaxLength(255);
        builder.Property(x => x.ReceiptCoefficient).HasPrecision(6, 2);
        builder.Property(x => x.DispatchCoefficient).HasPrecision(6, 2);
        builder.Property(x => x.PackagingCoefficient).HasPrecision(6, 2);
    }
}