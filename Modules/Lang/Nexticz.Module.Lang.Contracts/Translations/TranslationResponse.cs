using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Lang.Contracts.Translations;

public class TranslationResponse
{
    public required Guid Id { get; set; }
    public required string Module { get; set; }
    public required string Feature { get; set; }
    public required string Component { get; set; }
    public required string Slug { get; set; }
    public required string Name { get; set; }
    public required string Value { get; set; }
    public required EnumHelper.LanguageShortcutEnum LanguageShortcut { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }
    public string? UpdatedWith { get; set; }
}