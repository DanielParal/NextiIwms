using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods;


internal abstract class DeliveryMethodErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-settings-api-deliveryMethodService-";

    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationCodeIsRequired",
        "Kód způsobu dodání není vyplněn."
    );
    
    public static Error ValidationNameIsRequired => Error.Validation(
        ComponentSlug + "ValidationNameIsRequired",
        "Jméno způsobu dodání není vyplněno."
    );
    
    public static Error CodeDoesNotExist => Error.NotFound(
        ComponentSlug + "codeDoesNotExist",
        "Způsob dodání s tímto kódem neexistuje.");
    
    public static Error ValidationCodeDoesNotExist => Error.Validation(
        ComponentSlug + "validationCodeDoesNotExist",
        "Způsob dodání s tímto kódem neexistuje.");
    
    public static Error ValidationCodeAlreadyExists => Error.Validation(
        ComponentSlug + "validationCodeAlreadyExists",
        "Způsob dodání s tímto kódem již existuje."
    );
    
}