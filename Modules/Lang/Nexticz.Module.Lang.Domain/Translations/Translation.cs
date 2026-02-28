using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Lang.Domain.Languages;

namespace Nexticz.Module.Lang.Domain.Translations;

public class Translation
{
    public Guid Id { get; set; }
    public required string Module { get; set; }
    public required string Feature { get; set; }
    public required string Component { get; set; }
    public required string Name { get; set; }
    public required string Value { get; set; }
    public required string Slug { get; set; }
    public required EnumHelper.LanguageShortcutEnum LanguageShortcut { get; set; }
    public Language? Language { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Updated { get; set; }
    public string? UpdatedWith { get; set; }
}