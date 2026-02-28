using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Settings.Application.Workers;

internal abstract class WorkerErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-settings-api-workerService-";
    
    public static Error NotFoundWorkerWithPin => Error.NotFound(
        ComponentSlug + "NotFoundWorkerWithPin",
        "Uživatel s daným pinem nebyl nalezen."
    );
    
    public static Error NotFoundWorkerWithId => Error.NotFound(
        ComponentSlug + "NotFoundWorkerWithId",
        "Uživatel s ID nebyl nalezen."
    );
    
    public static Error ValidationWorkerWithPinAlreadyExist => Error.Validation(
        ComponentSlug + "ValidationWorkerWithPinAlreadyExist",
        "Uživatele s tímto pinem již existuje."
    );
    
    public static Error ValidationNameIsRequired => Error.Validation(
        ComponentSlug + "ValidationNameIsRequired",
        "Musíte vyplnit jméno uživatele."
    );
    
    public static Error ValidationPinHasToHaveBetween4And8Digits => Error.Validation(
        ComponentSlug + "ValidationPinHasToHaveBetween4And8Digits",
        "Pin musí mít minimálně 4 číslice a maximálně 8 číslic."
    );
}