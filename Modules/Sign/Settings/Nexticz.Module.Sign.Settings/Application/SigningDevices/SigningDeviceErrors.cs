using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices;


internal abstract class SigningDeviceErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-settings-api-signingDeviceService-";

    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "validationCodeIsRequired",
        "Kód podpisového zařízení není vyplněn."
    );
    
    public static Error ValidationNameIsRequired => Error.Validation(
        ComponentSlug + "validationNameIsRequired",
        "Jméno podpisového zařízení není vyplněno."
    );
    
    public static Error ValidationLocationCodeIsRequired => Error.Validation(
        ComponentSlug + "validationLocationCodeIsRequired",
        "Kód lokace není vyplněno."
    );
    
    public static Error ValidationPrinterCodeIsRequired => Error.Validation(
        ComponentSlug + "validationPrinterCodeIsRequired",
        "Kód tiskárny není vyplněno."
    );
    
    public static Error CodeDoesNotExist => Error.NotFound(
        ComponentSlug + "codeDoesNotExist",
        "Podpisové zařízení s tímto kódem neexistuje.");
    
    public static Error ValidationCodeDoesNotExist => Error.Validation(
        ComponentSlug + "validationCodeDoesNotExist",
        "Podpisové zařízení s tímto kódem neexistuje.");
    
    public static Error ValidationCodeAlreadyExists => Error.Validation(
        ComponentSlug + "validationCodeAlreadyExists",
        "Podpisové zařízení s tímto kódem již existuje."
    );
    
    public static Error ValidationLocationDoesNotExists => Error.Validation(
        ComponentSlug + "validationLocationDoesNotExists",
        "Lokace s tímto kódem neexistuje."
    );
    
    public static Error ValidationPrinterDoesNotExists => Error.Validation(
        ComponentSlug + "validationPrinterDoesNotExists",
        "Tiskárna s tímto kódem neexistuje."
    );
    
    public static Error ValidationSigningDeviceIsNotActive => Error.Validation(
        ComponentSlug + "ValidationSigningDeviceIsNotActive",
        "Podpisové zařízení není aktivní."
    );
    
    public static Error ValidationCodeIsAssignedToUsers(string userNames) => Error.Validation(
        ComponentSlug + "validationCodeIsAssignedToUsers",
        $"Nemůžeme smazat podpisové zařízení, protože je stále nastaveno u těchto uživatelů: {userNames}."
    );
    
}