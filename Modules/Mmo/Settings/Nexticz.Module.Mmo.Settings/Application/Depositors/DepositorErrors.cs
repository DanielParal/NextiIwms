using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Settings.Application.Depositors;


internal abstract class DepositorErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-settings-api-depositorService-";

    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "validationDepositorCodeIsRequired",
        "Kód ukladatele není vyplněn"
    );
    
    public static Error ValidationNameIsRequired => Error.Validation(
        ComponentSlug + "validationNameIsRequired",
        "Jméno ukladatele není vyplněno"
    );
    
    public static Error DepositorWithCodeDoesNotExist => Error.NotFound(
        ComponentSlug + "depositorWithCodeDoesNotExist",
        "Ukladatel s tímto kódem neexistuje");
    
    public static Error DepositorWithCodeAlreadyExists(string code) => Error.Validation(
        ComponentSlug + "depositorWithCodeAlreadyExists",
        $"Ukladatel s kódem {code} již existuje");
    
    public static Error ValidationCodeIsStillUsedEitherInKitsOrInPackagings(string kitCodes, string packagingCodes) => Error.Validation(
        ComponentSlug + "validationCodeIsUsedInKits",
        $"Ukladatel je stále použit buď v existujících kitech: {kitCodes} nebo v existujících obalech: {packagingCodes}");
}