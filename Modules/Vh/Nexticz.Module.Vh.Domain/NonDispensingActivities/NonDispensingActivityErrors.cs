using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Vh.Domain.NonDispensingActivities;

public abstract class NonDispensingActivityErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "vh-api-nonDispensingActivityService-";

    public static Error NonDispensingActivityWithIdDoesnotExist => Error.Validation(
        ComponentSlug + "nonDispensingActivityWithIdDoesnotExist",
        "Nevýdajová činnost s tímto ID neexistuje");
}