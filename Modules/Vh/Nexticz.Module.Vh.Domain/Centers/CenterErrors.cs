using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Vh.Domain.Centers;

public abstract class CenterErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "vh-api-centerService-";

    public static Error CenterWithIdDoesnotExist => Error.Validation(
        ComponentSlug + "centerWithIdDoesnotExist",
        "Středisko s tímto ID neexistuje");

    public static Error CreateCenterError => Error.Validation(
        ComponentSlug + "createCenterError",
        "Středisko se nepodařilo vytvořit");

    public static Error UpdateCenterError => Error.Validation(
        ComponentSlug + "updateCenterError",
        "Středisko se nepodařilo upravit");

    public static Error DeleteCenterError => Error.Validation(
        ComponentSlug + "deleteCenterError",
        "Středisko se nepodařilo smazat");
}