using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Lang.Domain.Translations;

namespace Nexticz.Module.Lang.Domain.Languages;

public class Language
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required EnumHelper.LanguageShortcutEnum Shortcut { get; set; }
    public required bool IsActive { get; set; }
    public ICollection<Translation>? Translations { get; set; }
}