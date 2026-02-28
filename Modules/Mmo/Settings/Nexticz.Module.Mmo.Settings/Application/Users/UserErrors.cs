using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Settings.Application.Users;

internal abstract class UserErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-settings-api-userService-";
    
    public static Error UserNotFound => Error.NotFound(
        ComponentSlug + "UserNotFound",
        "Uživatel nenalezen."
    );
    
    public static Error ValidationUserNameIsRequired => Error.Validation(
        ComponentSlug + "ValidationUserNameIsRequired",
        "Uživatelské jméno je povinné pole."
    );
    
    public static Error ValidationUserWithUserNameAlreadyExist => Error.Validation(
        ComponentSlug + "ValidationUserWithUserNameAlreadyExist",
        "Uživatele s tímto jménem již existuje."
    );
    
    public static Error ValidationUserWithUserNameDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationUserWithUserNameDoesNotExist",
        "Uživatele s tímto jménem neexistuje."
    );
}