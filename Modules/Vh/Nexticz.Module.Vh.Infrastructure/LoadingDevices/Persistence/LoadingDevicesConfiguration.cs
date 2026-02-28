using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Vh.Domain.LoadingDevices;

namespace Nexticz.Module.Vh.Infrastructure.LoadingDevices.Persistence;

public class LoadingDevicesConfiguration : IEntityTypeConfiguration<LoadingDevice>
{
    public void Configure(EntityTypeBuilder<LoadingDevice> builder)
    {
        builder.ToTable("LoadingDevices");
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.LastActivityWorkerCodeWms).HasMaxLength(100);
        builder
            .HasMany(x => x.LoadingActionsNdas)
            .WithOne(x => x.LoadingDevice)
            .HasForeignKey(x => x.LoadingDeviceId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}