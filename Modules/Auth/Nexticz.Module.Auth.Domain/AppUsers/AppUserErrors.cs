using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Auth.Domain.AppUsers;

public abstract class AppUserErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "auth-api-appUserService-";

    public static Error AppUserWithUsernameDoesNotExist => Error.Validation(
        ComponentSlug + "appUserWithUsernameDoesNotExist",
        "Uživatel s tímto Username neexistuje");

    public static Error AppUserWithIdDoesNotExist => Error.Validation(
        ComponentSlug + "appUserWithIdDoesNotExist",
        "Uživatel s tímto Id neexistuje");

    public static Error CreateAppUserError => Error.Validation(
        ComponentSlug + "createAppUserError",
        "Uživatele se nepodařilo vytvořit");

    public static Error UpdateAppUserError => Error.Validation(
        ComponentSlug + "updateAppUserError",
        "Uživatele se nepodařilo upravit");

    public static Error DeleteAppUserError => Error.Validation(
        ComponentSlug + "deleteAppUserError",
        "Uživatele se nepodařilo smazat");
    public static Error DeleteAppUserRefreshTokenError => Error.Validation(
        ComponentSlug + "deleteAppUserRefreshTokenError",
        "Zařízení se nepodařilo odhlásit");
}