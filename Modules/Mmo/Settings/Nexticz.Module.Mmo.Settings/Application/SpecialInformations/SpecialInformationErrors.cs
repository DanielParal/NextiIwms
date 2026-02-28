using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations;

internal abstract class SpecialInformationErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-settings-api-specialInformationService-";
    
    public static Error SpecialInformationNotFound => Error.NotFound(
        ComponentSlug + "SpecialInformationNotFound",
        "Speciální informace nenalezena"
    );
    
    public static Error ValidationTitleIsRequired => Error.Validation(
        ComponentSlug + "ValidationTitleIsRequired",
        "Titulek není vyplněn"
    );
    
    public static Error ValidationDescriptionIsRequired => Error.Validation(
        ComponentSlug + "ValidationDescriptionIsRequired",
        "Popis není vyplněn"
    );
    
    public static Error ValidationSpecialInformationWithTitleAlreadyExists => Error.Validation(
        ComponentSlug + "ValidationSpecialInformationWithTitleAlreadyExists",
        "Speciální informace s tímto titulkem již existuje"
    );
    
    public static Error ValidationSpecialInformationWithIdNotFound => Error.Validation(
        ComponentSlug + "ValidationSpecialInformationWithIdNotFound",
        "Speciální informace s tímto ID neexistuje"
    );
    
    public static Error NotFoundSpecialInformationFile => Error.NotFound(
        ComponentSlug + "NotFoundSpecialInformationFile",
        "Obrázek pro speciální informace nebyl nalezen."
    );
    
    public static Error ValidationSpecialInformationIsStillInUseInKits(string kits) => Error.NotFound(
        ComponentSlug + "ValidationSpecialInformationIsStillInUseInKits",
        $"Speciální informace je stále přiřazena v kitech: {kits}."
    );
}