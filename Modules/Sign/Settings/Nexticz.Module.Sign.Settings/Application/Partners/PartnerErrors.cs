using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.Settings.Application.Partners;


internal abstract class PartnerErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-settings-api-partnerService-";

    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "validationCodeIsRequired",
        "Kód partnera není vyplněn."
    );
    
    public static Error ValidationNameIsRequired => Error.Validation(
        ComponentSlug + "validationNameIsRequired",
        "Jméno partnera není vyplněno."
    );
    
    public static Error CodeDoesNotExist => Error.NotFound(
        ComponentSlug + "codeDoesNotExist",
        "Partner s tímto kódem neexistuje.");
    
    public static Error ValidationCodeDoesNotExist => Error.Validation(
        ComponentSlug + "validationCodeDoesNotExist",
        "Partner s tímto kódem neexistuje.");
    
    public static Error ValidationCodeAlreadyExists => Error.Validation(
        ComponentSlug + "validationCodeAlreadyExists",
        "Partner s tímto kódem již existuje."
    );
    
    public static Error ValidationCodeIsUsedInReceivers(string receiverCodes) => Error.Validation(
        ComponentSlug + "validationCodeIsUsedInReceivers",
        $"Nemůžeme smazat partnera, protože je stále nastaven u těchto příjemců: {receiverCodes}."
    );
}