using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Vh.Domain.LoadingActionsNdas;

public abstract class LoadingActionsNdaErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "vh-api-loadingActionsNdaService-";

    public static Error LoadingActionsNdaWithIdDoesnotExist => Error.Validation(
        ComponentSlug + "loadingActionsNdaWithIdDoesnotExist",
        "Načítací akce Nda s tímto ID neexistuje");
}