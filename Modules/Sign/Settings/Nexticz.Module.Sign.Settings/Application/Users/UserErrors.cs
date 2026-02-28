using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.Settings.Application.Users;


internal abstract class UserErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-settings-api-userService-";
    
    public static Error ValidationUserNameIsRequired => Error.Validation(
        ComponentSlug + "validationUserNameIsRequired",
        "Jméno uživatele je povinné pole."
    );
    
    public static Error UserNameNotFound => Error.NotFound(
        ComponentSlug + "userNameNotFound",
        "Uživatel s tímto uživatelským jménem neexistuje.");
    
    public static Error ValidationUserNameDoesNotExist => Error.Validation(
        ComponentSlug + "validationUserNameDoesNotExist",
        "Uživatel s tímto uživatelským jménem neexistuje.");
    
    public static Error ValidationUserNameAlreadyExists => Error.Validation(
        ComponentSlug + "validationUserNameAlreadyExists",
        "Uživatel s tímto uživatelským jménem již existuje."
    );
    
    public static Error ValidationDidNotFindAllDepositors => Error.Validation(
        ComponentSlug + "validationDidNotFindAllDepositors",
        "Nenašli jsme všechny ukladatele v databázi."
    );
    
    public static Error ValidationDidNotFindAllDepositorGroups => Error.Validation(
        ComponentSlug + "validationDidNotFindAllDepositorGroups",
        "Nenašli jsme všechny skupiny ukladatelů v databázi."
    );
    
    public static Error ValidationDidNotFindAllSigningDevices => Error.Validation(
        ComponentSlug + "validationDidNotFindAllSigningDevices",
        "Nenašli jsme všechny podepisovací zařízení v databázi."
    );
    
    public static Error ValidationDidNotFindAllPrinters => Error.Validation(
        ComponentSlug + "ValidationDidNotFindAllPrinters",
        "Nenašli jsme všechny tiskárny v databázi."
    );
    
    public static Error NotFoundUserSignatureFile => Error.NotFound(
        ComponentSlug + "NotFoundUserSignatureFile",
        "Podpis pro tohoto uživatele nebyl nalezen."
    );
}