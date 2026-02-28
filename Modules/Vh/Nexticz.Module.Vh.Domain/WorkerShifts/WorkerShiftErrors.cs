using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Vh.Domain.WorkerShifts;

public abstract class WorkerShiftErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "vh-api-workerShiftsService-";

    public static Error WorkerShiftWithIdDoesNotExist => Error.Validation(
        ComponentSlug + "workerShiftWithIdDoesNotExist",
        "Pracovní směna s tímto ID neexistuje");

    public static Error AddWorkerShiftError => Error.Validation(
        ComponentSlug + "addWorkerShiftError",
        "Pracovní směna se nepodařila přidat");
}