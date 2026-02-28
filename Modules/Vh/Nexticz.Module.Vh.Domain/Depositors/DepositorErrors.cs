using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Vh.Domain.Depositors;

public abstract class DepositorErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "vh-api-depositorService-";

    public static Error DepositorWithIdDoesnotExist => Error.Validation(
        ComponentSlug + "depositorWithIdDoesnotExist",
        "Ukladatel s tímto ID neexistuje");

    public static Error CreateDepositorError => Error.Validation(
        ComponentSlug + "createDepositorError",
        "Ukladatele se nepodařilo vytvořit");

    public static Error UpdateDepositorError => Error.Validation(
        ComponentSlug + "updateDepositorError",
        "Ukladatele se nepodařilo upravit");

    public static Error DeleteDepositorError => Error.Validation(
        ComponentSlug + "deleteDepositorError",
        "Ukladatele se nepodařilo smazat");
}