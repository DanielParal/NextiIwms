using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.Depositors;

namespace Nexticz.Module.Vh.Infrastructure.Depositors.Persistence;

public class DepositorsConfiguration : IEntityTypeConfiguration<Depositor>
{
    public void Configure(EntityTypeBuilder<Depositor> builder)
    {
        builder.ToTable("Depositors");
        builder.Property(x => x.Name).HasMaxLength(50);
        builder.Property(x => x.Code).HasMaxLength(50);
    }
}