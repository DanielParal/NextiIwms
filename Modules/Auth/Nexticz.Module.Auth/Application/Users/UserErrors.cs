using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Auth.Application.Users;

internal abstract class UserErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "auth-module-users-application-";
    
    public static Error ValidationUserWithIdDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationUserWithIdDoesNotExist",
        "Uživatel s tímto Id neexistuje");
    
    public static Error ValidationUserWithEmailDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationUserWithEmailDoesNotExist",
        "Uživatel s tímto emailem neexistuje");
    
    public static Error ValidationUserWithUsernameDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationUserWithUsernameDoesNotExist",
        "Uživatel s tímto uživatelským jménem neexistuje");
    
    public static Error ValidationUserWithEmailAlreadyExists => Error.Validation(
        ComponentSlug + "ValidationUserWithEmailAlreadyExists",
        "Uživatel s tímto emailem již existuje");
    
    public static Error ValidationUserWithUserNameAlreadyExists => Error.Validation(
        ComponentSlug + "ValidationUserWithUserNameAlreadyExists",
        "Uživatel s tímto uživatelským jménem již existuje");
}