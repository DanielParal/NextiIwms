using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Auth.Infrastructure.Dbs.Tables;

namespace Nexticz.Module.Auth.Infrastructure.Dbs.TableConfigurations;

public class EventLogConfiguration : IEntityTypeConfiguration<EventLog>
{
    public void Configure(EntityTypeBuilder<EventLog> builder)
    {
        builder.ToTable("EventLogs");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.StreamId)
            .IsRequired();

        builder.Property(x => x.Data)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasMaxLength(200);

        builder.Property(x => x.DotnetType)
            .HasMaxLength(500);

        builder.Property(x => x.CorrelationId)
            .HasMaxLength(200);
        
        builder.Property(x => x.DateCreated)
            .IsRequired();

        builder.HasIndex(x => x.StreamId);
    }

}