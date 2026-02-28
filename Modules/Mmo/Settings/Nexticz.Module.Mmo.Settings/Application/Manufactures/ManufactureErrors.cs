using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Settings.Application.Manufactures;

internal abstract class ManufactureErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-settings-api-manufactureService-";
    
    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "validationManufactureCodeIsRequired",
        "Výroba není vyplněna"
    );
    
    public static Error ValidationNameIsRequired => Error.Validation(
        ComponentSlug + "validationNameIsRequired",
        "Jméno výroby není vyplněno"
    );
    
    public static Error CodeDoesNotExist => Error.NotFound(
        ComponentSlug + "manufactureWithCodeDoesNotExist",
        "Výroba s tímto kódem neexistuje");
    
    public static Error ValidationCodeAlreadyExists(string code) => Error.Validation(
        ComponentSlug + "manufactureWithCodeAlreadyExists",
        $"Výroba s kódem '{code}' již existuje");
    
    public static Error ValidationCodeIsStillUsedInKits(string codes) => Error.Validation(
        ComponentSlug + "validationCodeIsUsedInKits",
        $"Typ kitu je stále použit v existujících kitech: {codes}");
}