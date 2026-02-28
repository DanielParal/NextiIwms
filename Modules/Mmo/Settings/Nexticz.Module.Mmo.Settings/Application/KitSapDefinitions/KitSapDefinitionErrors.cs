using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions;

internal abstract class KitSapDefinitionErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-settings-api-kitSapDefinitionService-";
    
    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "validationCodeIsRequired",
        "Definice SAPu kitu není vyplněna"
    );
    
    public static Error ValidationNameIsRequired => Error.Validation(
        ComponentSlug + "validationNameIsRequired",
        "Definice SAPu kitu není vyplněna"
    );
    
    public static Error CodeDoesNotExist => Error.NotFound(
        ComponentSlug + "kitTypeWithCodeDoesNotExist",
        "Definice SAPu kitu s tímto kódem neexistuje");
    
    public static Error ValidationCodeAlreadyExists(string code) => Error.Validation(
        ComponentSlug + "ValidationCodeAlreadyExists",
        $"Definice SAPu kitu s kódem '{code}' již existuje");
    
    public static Error ValidationCodeIsStillUsedInKits(string codes) => Error.Validation(
        ComponentSlug + "ValidationCodeIsStillUsedInKits",
        $"Definice SAPu kitu je stále použit v existujících kitech: {codes}");
}