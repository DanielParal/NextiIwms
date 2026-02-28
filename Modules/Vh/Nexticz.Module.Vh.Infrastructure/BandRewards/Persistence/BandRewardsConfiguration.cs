using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.BandRewards;

namespace Nexticz.Module.Vh.Infrastructure.BandRewards.Persistence;

public class BandRewardsConfiguration : IEntityTypeConfiguration<BandReward>
{
    public void Configure(EntityTypeBuilder<BandReward> builder)
    {
        builder.ToTable("BandRewards");
        builder.Property(x => x.Band).HasMaxLength(50);
    }
}