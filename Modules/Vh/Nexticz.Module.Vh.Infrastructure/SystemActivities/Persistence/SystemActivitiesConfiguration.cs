using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.SystemActivities;

namespace Nexticz.Module.Vh.Infrastructure.SystemActivities.Persistence;

public class SystemActivitiesConfiguration : IEntityTypeConfiguration<SystemActivity>
{
    public void Configure(EntityTypeBuilder<SystemActivity> builder)
    {
        builder.ToTable("SystemActivities");
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.SystemType).HasMaxLength(100);
        builder.Property(x => x.ActionCodeWms).HasMaxLength(100);
        builder.Property(x => x.Type).HasMaxLength(100);
        builder.Property(x => x.WhatToMeasure).HasMaxLength(255);
        builder.Property(x => x.Unit).HasMaxLength(100);
        builder.Property(x => x.Coefficient).HasPrecision(6, 2);
    }
}