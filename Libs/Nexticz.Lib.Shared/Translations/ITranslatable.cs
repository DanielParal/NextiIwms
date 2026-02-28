namespace Nexticz.Lib.Shared.Translations;

public interface ITranslatable
{
    string TranslationKey { get; }
    string TranslationValue { get; }
}