using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.ActivityCategories;

namespace Nexticz.Module.Vh.Infrastructure.ActivityCategories.Persistence;

public class ActivityCategoriesConfiguration : IEntityTypeConfiguration<ActivityCategory>
{
    public void Configure(EntityTypeBuilder<ActivityCategory> builder)
    {
        builder.ToTable("ActivityCategories");
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Color).HasMaxLength(100);
        builder.Property(x => x.ActivityType).HasConversion<string>();
        builder
            .HasMany(x => x.NonDispensingActivities)
            .WithOne(x => x.ActivityCategory)
            .HasForeignKey(x => x.ActivityCategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasMany(x => x.SystemActivities)
            .WithOne(x => x.ActivityCategory)
            .HasForeignKey(x => x.ActivityCategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}