using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Portal.Application.Modules;

internal abstract class ModuleErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "portal-application-modulesService-";
    
    public static Error ValidationModuleWithIdDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationModuleDoesNotExist",
        "Modul s tímto ID neexistuje."
    );
    
    public static Error ModuleWithIdNotFound => Error.NotFound(
        ComponentSlug + "ModuleWithIdNotFound",
        "Modul s tímto ID nebyl nalezen."
    );
    
    public static Error ValidationModuleWithNameAlreadyExists => Error.Validation(
        ComponentSlug + "ValidationModuleWithNameAlreadyExists",
        "Modul s tímto jménem již existuje."
    );
    
    public static Error ValidationModuleWithNameDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationModuleWithNameDoesNotExist",
        "Modul s tímto jménem neexistuje."
    );
    
    public static Error ValidationCannotMoveItHigherThanNumberOfModules => Error.Validation(
        ComponentSlug + "ValidationCannotMoveItHigherThanNumberOfModules",
        "Špátné číslo pořadí. Modul nemůže být posunut dál, než je počet modulů."
    );
}