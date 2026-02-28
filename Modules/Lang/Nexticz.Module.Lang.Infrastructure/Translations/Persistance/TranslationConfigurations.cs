using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Lang.Domain.Translations;

namespace Nexticz.Module.Lang.Infrastructure.Translations.Persistance;

public class TranslationConfigurations : IEntityTypeConfiguration<Translation>
{
    public void Configure(EntityTypeBuilder<Translation> builder)
    {
        builder.ToTable(name: "Translations");
        builder.Property(x => x.Module).HasMaxLength(50);
        builder.Property(x => x.Feature).HasMaxLength(50);
        builder.Property(x => x.Component).HasMaxLength(50);
        builder.Property(x => x.Slug).HasMaxLength(255);
        builder.Property(x => x.Name).HasMaxLength(50);
        builder.Property(x => x.Value).HasMaxLength(255);
        builder.Property(x => x.UpdatedWith).HasMaxLength(255);
    }
}