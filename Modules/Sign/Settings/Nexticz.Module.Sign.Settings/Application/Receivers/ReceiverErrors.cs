using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.Settings.Application.Receivers;


internal abstract class ReceiverErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-settings-api-receiverService-";

    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "validationCodeIsRequired",
        "Kód příjemce není vyplněn."
    );
    
    public static Error ValidationPartnerCodeIsRequired => Error.Validation(
        ComponentSlug + "validationPartnerCodeIsRequired",
        "Kód partnera není vyplněn."
    );
    
    public static Error ValidationNameIsRequired => Error.Validation(
        ComponentSlug + "validationNameIsRequired",
        "Jméno příjemce není vyplněno."
    );
    
    public static Error CombinationCodeAndPartnerCodeNotFound => Error.NotFound(
        ComponentSlug + "combinationCodeAndPartnerCodeNotFound",
        "Příjemce s tímto kódem a kódem partnera nenalezen.");
    
    public static Error ValidationCombinationCodeAndPartnerCodeDoesNotExist => Error.Validation(
        ComponentSlug + "validationCombinationCodeAndPartnerCodeDoesNotExist",
        "Partner s tímto kódem a kódem partnera neexistuje.");
    
    public static Error ValidationCombinationCodeAndPartnerCodeAlreadyExists => Error.Validation(
        ComponentSlug + "validationCombinationCodeAndPartnerCodeAlreadyExists",
        "Partner s tímto kódem a kódem partnera již existuje."
    );
    
    public static Error ValidationPartnerDoesNotExist => Error.Validation(
        ComponentSlug + "validationPartnerDoesNotExists",
        "Partner s tímto kódem neexistuje."
    );
    
}