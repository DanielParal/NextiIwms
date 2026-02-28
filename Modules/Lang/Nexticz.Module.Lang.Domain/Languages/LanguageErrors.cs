using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Lang.Domain.Languages;

public abstract class LanguageErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "lang-api-languageService-";

    public static Error LanguageWithShortcutDoesnotExist => Error.Validation(
        code: ComponentSlug + "languageWithShortcutDoesnotExist",
        description: "Tento jazyk neexistuje");
    
    public static Error CreateLanguageError => Error.Validation(
        code: ComponentSlug + "createLanguageError",
        description: "Jazyk se nepodařilo vytvořit");
    
    public static Error UpdateLanguageError => Error.Validation(
        code: ComponentSlug + "updateLanguageError",
        description: "Jazyk se nepodařilo upravit");
}