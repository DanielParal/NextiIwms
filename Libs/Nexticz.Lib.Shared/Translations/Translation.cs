namespace Nexticz.Lib.Shared.Translations;

public record Translation(string TranslationKey, string TranslationValue) : ITranslatable;