using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.Settings.Application.Locations;


internal abstract class LocationErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-settings-api-locationService-";

    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "validationCodeIsRequired",
        "Kód lokace není vyplněn."
    );
    
    public static Error ValidationNameIsRequired => Error.Validation(
        ComponentSlug + "validationNameIsRequired",
        "Jméno lokace není vyplněno."
    );
    
    public static Error CodeDoesNotExist => Error.NotFound(
        ComponentSlug + "codeDoesNotExist",
        "Lokace s tímto kódem neexistuje.");
    
    public static Error ValidationCodeDoesNotExist => Error.Validation(
        ComponentSlug + "validationCodeDoesNotExist",
        "Lokace s tímto kódem neexistuje.");
    
    public static Error ValidationCodeAlreadyExists => Error.Validation(
        ComponentSlug + "validationCodeAlreadyExists",
        "Lokace s tímto kódem již existuje."
    );
    
    public static Error ValidationCodeIsUsedInSigningDevices(string signingDevicesCodes) => Error.Validation(
        ComponentSlug + "validationCodeIsUsedInSigningDevices",
        $"Nemůžeme smazat tuto lokaci, protože je stále nastavena v těchto podpisových zařízeních: {signingDevicesCodes}."
    );
}