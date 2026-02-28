using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Vh.Domain.DepositorsGroups;

public abstract class DepositorsGroupErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "vh-api-depositorsGroupsService-";

    public static Error DepositorsGroupWithIdDoesnotExist => Error.Validation(
        ComponentSlug + "depositorsGroupWithIdDoesnotExist",
        "Skupina ukladatelů s tímto ID neexistuje");

    public static Error CreateDepositorsGroupError => Error.Validation(
        ComponentSlug + "createDepositorsGroupError",
        "Skupinu ukladatelů se nepodařilo vytvořit");

    public static Error UpdateDepositorsGroupError => Error.Validation(
        ComponentSlug + "updateDepositorsGroupError",
        "Skupinu ukladatelů se nepodařilo upravit");

    public static Error DeleteDepositorsGroupError => Error.Validation(
        ComponentSlug + "deleteDepositorsGroupError",
        "Skupinu ukladatelů se nepodařilo smazat");
}