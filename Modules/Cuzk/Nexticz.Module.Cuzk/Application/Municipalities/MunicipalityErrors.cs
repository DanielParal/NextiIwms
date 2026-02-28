using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Cuzk.Application.Municipalities;

internal abstract class MunicipalityErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "minucipality-application-modulesService-";
    
    public static Error ValidationMunicipalityWithCodeAlreadyExists => Error.Validation(
        ComponentSlug + "ValidationMunicipalityWithCodeAlreadyExists",
        "Samospráva s tímto kódem již existuje."
    );
    
    public static Error ValidationMunicipalityWithCodeDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationMunicipalityWithCodeAlreadyExists",
        "Samospráva s tímto kódem již neexistuje."
    );
    
    public static Error ValidationMunicipalityCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationMunicipalityCodeIsRequired",
        "Kód samosprávy je povinné pole."
    );
}