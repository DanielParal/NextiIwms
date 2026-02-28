using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Washing.Application.LastEnteredWorkerOnLines;

internal abstract class LastEnteredWorkerOnLineErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-washing-api-lastEnteredWorkerService-";
    
    public static Error WorkerNotFound => Error.NotFound(
        ComponentSlug + "WorkerNotFound",
        "Pracovník nebyl nalezen."
    );
    
    public static Error ValidationWorkerNotFound => Error.Validation(
        ComponentSlug + "ValidationWorkerNotFound",
        "Pracovník nebyl nalezen."
    );
    
    public static Error ValidationWorkerIsNotActive => Error.Validation(
        ComponentSlug + "ValidationWorkerIsNotActive",
        "Pracovník není aktivní."
    );
    
    public static Error ValidationLineCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationLineCodeIsRequired",
        "Kód lajny je povinné pole."
    );
    
    public static Error ValidationNoUserOnTheLine => Error.Validation(
        ComponentSlug + "ValidationNoUserOnTheLine",
        "žádný uživatel není přihlášen na této lajně."
    );
}