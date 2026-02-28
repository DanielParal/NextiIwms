using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Auth.Infrastructure.Identity;

public abstract class IdentityPasswordValidationErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "auth-passwordValidation-infrastructure-";
    
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
        "Heslo musí obsahovat unikátní znaky");

    public static Error DuplicateEmail => Error.Validation(
        ComponentSlug + "duplicateEmail",
        "Uživatel s tímto e-mailem je již zaregistrovaný");

    public static Error InvalidEmail => Error.Validation(
        ComponentSlug + "invalidEmail",
        "Nebyl zadán validní e-mail");
}