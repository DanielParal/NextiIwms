using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexticz.Module.Lang.Domain.Languages;

namespace Nexticz.Module.Lang.Infrastructure.Languages.Persistance;

public class LanguageConfigurations : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable(name: "Languages");
        builder.Property(x => x.Name).HasMaxLength(10);
        builder.Property(x => x.Shortcut).HasConversion<string>().HasMaxLength(10);
        builder
            .HasMany(x => x.Translations)
            .WithOne(x => x.Language)
            .HasForeignKey(x => x.LanguageShortcut)
            .HasPrincipalKey(x => x.Shortcut)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}