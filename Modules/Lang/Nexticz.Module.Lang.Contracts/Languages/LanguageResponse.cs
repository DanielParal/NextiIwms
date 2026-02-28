using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Lang.Contracts.Languages;

public class LanguageResponse
{
    public required string Name { get; set; }
    public required EnumHelper.LanguageShortcutEnum Shortcut { get; set; }
    public required bool IsActive { get; set; }
}