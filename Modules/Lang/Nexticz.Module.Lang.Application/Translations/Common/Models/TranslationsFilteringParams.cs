using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Lang.Application.Translations.Common.Models;

public class TranslationsFilteringParams : BaseFilteringParams
{
    public string? Slug { get; set; }
    public string? Module { get; set; }
    public string? Feature { get; set; }
    public string? Component { get; set; }
    public EnumHelper.LanguageShortcutEnum? LanguageShortcut { get; set; }
}