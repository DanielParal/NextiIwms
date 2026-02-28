using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingCirculations;

internal abstract class PackagingCirculationErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-settings-api-packagingCirculationService-";

    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "validationPackagingCirculationCodeIsRequired",
        "Oběhovost obalu není vyplněno"
    );
    
    public static Error ValidationNameIsRequired => Error.Validation(
        ComponentSlug + "validationNameIsRequired",
        "Jméno oběhovosti obalu není vyplněno"
    );

    public static Error CodeDoesNotExist => Error.NotFound(
        ComponentSlug + "packagingCirculationWithCodeDoesNotExist",
        "Oběhovost obalu s tímto kódem neexistuje"
    );

    public static Error ValidationCodeAlreadyExists(string code) => Error.Validation(
        ComponentSlug + "packagingCirculationWithCodeAlreadyExists",
        $"Oběhovost obalu s kódem '{code}' již existuje"
    );
    
    public static Error ValidationCodeIsStillUsedInPackagings(string codes) => Error.Validation(
        ComponentSlug + "validationCodeIsUsedInPackagings",
        $"Oběhovost obalu je stále použita v existujících obalech: {codes}");
}