using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Lang.Domain.Translations;

public abstract class TranslationErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "lang-api-translationService-";

    public static Error TranslationWithIdDoesnotExist => Error.Validation(
        code: ComponentSlug + "translationWithIdDoesnotExist",
        description: "Tento překlad neexistuje");
    
    public static Error CreateTranslationError => Error.Validation(
        code: ComponentSlug + "createTranslationError",
        description: "Překlad se nepodařilo vytvořit");
    
    public static Error UpdateTranslationError => Error.Validation(
        code: ComponentSlug + "updateTranslationError",
        description: "Překlad se nepodařilo upravit");
}