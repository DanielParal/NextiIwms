using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Lib.Shared.Contracts;

public class CreateTranslationsRequest
{
    public required List<CreateTranslationsItem> Items { get; set; }
    public required EnumHelper.LanguageShortcutEnum LanguageShortcut { get; set; }
}

public class CreateTranslationsItem
{
    public required string Slug { get; set; }
    public required string Value { get; set; }
}