using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Vh.Domain.SystemActivities;

public abstract class SystemActivityErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "vh-api-systemActivityService-";

    public static Error SystemActivityWithIdDoesnotExist => Error.Validation(
        ComponentSlug + "systemActivityWithIdDoesnotExist",
        "Systémová činnost s tímto ID neexistuje");
}