using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Settings.Application.KitTypes;

internal abstract class KitTypeErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-settings-api-kitTypeService-";
    
    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "validationCodeIsRequired",
        "Kód typu kitu není vyplněn"
    );
    
    public static Error ValidationNameIsRequired => Error.Validation(
        ComponentSlug + "validationNameIsRequired",
        "Jméno typu kitu není vyplněno"
    );
    
    public static Error CodeDoesNotExist => Error.NotFound(
        ComponentSlug + "kitTypeWithCodeDoesNotExist",
        "Typ kitu s tímto kódem neexistuje");
    
    public static Error ValidationCodeAlreadyExists(string code) => Error.Validation(
        ComponentSlug + "validationKitTypeWithCodeAlreadyExists",
        $"Typ kitu s kódem '{code}' již existuje");
    
    public static Error ValidationCodeIsStillUsedInKits(string codes) => Error.Validation(
        ComponentSlug + "validationCodeIsUsedInKits",
        $"Typ kitu je stále použit v existujících kitech: {codes}");
}