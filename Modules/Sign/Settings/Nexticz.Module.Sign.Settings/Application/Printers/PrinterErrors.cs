using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.Settings.Application.Printers;


internal abstract class PrinterErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-settings-api-printerService-";

    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "validationCodeIsRequired",
        "Kód tiskárny není vyplněn."
    );
    
    public static Error ValidationNameIsRequired => Error.Validation(
        ComponentSlug + "validationNameIsRequired",
        "Jméno tiskárny není vyplněno."
    );
    
    public static Error ValidationIpIsRequired => Error.Validation(
        ComponentSlug + "validationIpIsRequired",
        "IP tiskárny není vyplněno."
    );
    
    public static Error CodeDoesNotExist => Error.NotFound(
        ComponentSlug + "codeDoesNotExist",
        "Tiskárna s tímto kódem neexistuje.");
    
    public static Error ValidationCodeDoesNotExist => Error.Validation(
        ComponentSlug + "validationCodeDoesNotExist",
        "Tiskárna s tímto kódem neexistuje.");
    
    public static Error ValidationCodeAlreadyExists => Error.Validation(
        ComponentSlug + "validationCodeAlreadyExists",
        "Tiskárna s tímto kódem již existuje."
    );
    
    public static Error ValidationCodeIsUsedInSigningDevices(string signingDevicesCodes) => Error.Validation(
        ComponentSlug + "validationCodeIsUsedInSigningDevices",
        $"Nemůžeme smazat tuto tiskárnu, protože je stále nastavena v těchto podpisových zařízeních: {signingDevicesCodes}."
    );
}