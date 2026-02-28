using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Vh.Domain.VhUsers;

public abstract class VhUserErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "vh-api-vhUserService-";

    public static Error VhUserWithIdDoesnotExist => Error.Validation(
        ComponentSlug + "vhUserWithIdDoesnotExist",
        "VhUser s tímto ID neexistuje");

    public static Error CreateVhUserError => Error.Validation(
        ComponentSlug + "createVhUserError",
        "VhUser se nepodařil vytvořit");

    public static Error UpdateVhUserError => Error.Validation(
        ComponentSlug + "updateVhUserError",
        "VhUser se nepodařil upravit");

    public static Error DeleteVhUserError => Error.Validation(
        ComponentSlug + "deleteVhUserError",
        "VhUser se nepodařil smazat");
}