using ErrorOr;

namespace Nexticz.Module.Mmo.Reporting.Application.WorkerLinePresences;

internal abstract class WorkerLinePresenceErrors
{
    public static string ComponentSlug { get; } = "mmo-reporting-api-workerLinePresenceService-";
    
    public static Error ValidationLineCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationLineCodeIsRequired",
        "Kód lajny je povinné pole."
    );
}