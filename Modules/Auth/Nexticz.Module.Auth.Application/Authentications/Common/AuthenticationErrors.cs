using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Auth.Application.Authentications.Common;

public abstract class AuthenticationErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "auth-api-authService-";

    public static Error UserIsCurentlyLogged => Error.Validation(
        ComponentSlug + "userIsCurentlyLogged",
        "Neočekávaná chyba, byli jste odhlášeni");

    public static Error ValidationTokenError => Error.Unauthorized(
        ComponentSlug + "validationTokenError",
        "JWT token není validní");

    public static Error RegistrationError => Error.Validation(
        ComponentSlug + "registrationError",
        "Chyba při registraci");

    public static Error UsernameEmptyString => Error.Validation(
        ComponentSlug + "usernameEmptyString",
        "Uživatelské jméno nesmí obsahovat prázdné znaky");

    public static Error UsernameMinimalLength => Error.Validation(
        ComponentSlug + "usernameMinimalLength",
        "Uživatelské jméno musí obsahovat minimálně {0} znaky");

    public static Error InvalidUsername => Error.Validation(
        ComponentSlug + "invalidUsername",
        "Uživatelské jméno není platné");

    public static Error InvalidPassword => Error.Validation(
        ComponentSlug + "invalidPassword",
        "Neplatné heslo");

    public static Error PasswordEmptyString => Error.Validation(
        ComponentSlug + "passwordEmptyString",
        "Heslo nesmí obsahovat prázdné znaky");

    public static Error PasswordTooShort => Error.Validation(
        ComponentSlug + "passwordTooShort",
        "Heslo je příliš krátké, musí být alespoň 8 znaků");

    public static Error PasswordRequiresNonAlphanumeric => Error.Validation(
        ComponentSlug + "passwordRequiresNonAlphanumeric",
        "Heslo musí obsahovat speciální znak");

    public static Error PasswordRequiresDigit => Error.Validation(
        ComponentSlug + "passwordRequiresDigit",
        "Heslo musí obsahovat číslici");

    public static Error PasswordRequiresUpper => Error.Validation(
        ComponentSlug + "passwordRequiresUpper",
        "Heslo musí obsahovat velké písmeno");

    public static Error PasswordRequiresLower => Error.Validation(
        ComponentSlug + "passwordRequiresLower",
        "Heslo musí obsahovat malé písmeno");

    public static Error PasswordUnspecifiedError => Error.Validation(
        ComponentSlug + "passwordUnspecifiedError",
        "Heslo obsahuje nespecifikovanou chybu");

    public static Error PasswordRequiresUniqueChars => Error.Validation(
        ComponentSlug + "passwordRequiresUniqueChars",
        "Heslo musí obsahovat alespoň 4 unikátní znaky");

    public static Error UserExist => Error.Validation(
        ComponentSlug + "userExist",
        "Uživatelské jméno nebo e-mail je již zaregistrované");

    public static Error DuplicateEmail => Error.Validation(
        ComponentSlug + "duplicateEmail",
        "Uživatel s tímto e-mailem je již zaregistrovaný");

    public static Error InvalidEmail => Error.Validation(
        ComponentSlug + "invalidEmail",
        "Nebyl zadán validní e-mail");

    public static Error EmailConfirmationTokenError => Error.Validation(
        ComponentSlug + "emailConfirmationTokenError",
        "Špatný token");

    public static Error EmailConfirmationEmailError => Error.Validation(
        ComponentSlug + "emailConfirmationEmailError",
        "Špatný e-mail");

    public static Error UserIsBlockedError => Error.Validation(
        ComponentSlug + "userIsBlockedError",
        "Uživatel je blokován");
}