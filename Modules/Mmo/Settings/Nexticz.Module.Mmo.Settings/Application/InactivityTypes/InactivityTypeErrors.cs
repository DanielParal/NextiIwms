using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Settings.Application.InactivityTypes;

internal abstract class InactivityTypeErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-settings-api-inactivityTypeService-";
    
    public static Error NotFoundInactivityTypeWithId => Error.NotFound(
        ComponentSlug + "NotFoundInactivityTypeWithId",
        "Typ neaktivity s tímto ID nebyl nalezen."
    );
    
    public static Error NotFoundInactivityTypeWithName => Error.NotFound(
        ComponentSlug + "NotFoundInactivityTypeWithName",
        "Typ neaktivity s tímto jménem nebyl nalezen."
    );
    
    public static Error ValidationInactivityTypeWithNameAlreadyExist => Error.Validation(
        ComponentSlug + "ValidationInactivityTypeWithNameAlreadyExist",
        "Typ neaktivity s tímto jménem již existuje."
    );
    
    public static Error ValidationNameIsRequired => Error.Validation(
        ComponentSlug + "ValidationNameIsRequired",
        "Musíte vyplnit jméno typu."
    );
}