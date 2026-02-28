using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Vh.Domain.Workers;

public abstract class WorkerErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "vh-api-workerService-";

    public static Error WorkerWithIdDoesnotExist => Error.Validation(
        ComponentSlug + "workerWithIdDoesnotExist",
        "Pracovník s tímto ID neexistuje");
}