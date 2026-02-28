using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingTypes;

internal abstract class PackagingTypeErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-settings-api-packagingTypeService-";
    
    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "validationCodeIsRequired",
        "Kód typu obalu není vyplněn"
    );
    
    public static Error ValidationNameIsRequired => Error.Validation(
        ComponentSlug + "validationNameIsRequired",
        "Jméno typu obalu není vyplněno"
    );
    
    public static Error CodeDoesNotExist => Error.NotFound(
        ComponentSlug + "packageTypeWithCodeDoesNotExist",
        "Typ obalu s tímto kódem neexistuje");
    
    public static Error ValidationCodeAlreadyExists(string code) => Error.Validation(
        ComponentSlug + "packagingTypeWithCodeAlreadyExists",
        $"Typ obalu s kódem '{code}' již existuje");
    
    public static Error ValidationCodeIsStillUsedInPackagings(string codes) => Error.Validation(
        ComponentSlug + "validationCodeIsUsedInPackagings",
        $"Typ obalu je stále použit v existujících obalech: {codes}");
}