using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.Settings.Application.Depositors;


internal abstract class DepositorErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-settings-api-depositorService-";

    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "validationCodeIsRequired",
        "Kód ukladatele není vyplněn."
    );
    
    public static Error ValidationDepositorGroupCodeIsRequired => Error.Validation(
        ComponentSlug + "validationDepositorGroupCodeIsRequired",
        "Kód skupiny ukladatelů není vyplněn."
    );
    
    public static Error ValidationDepositorGroupDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationDepositorGroupDoesNotExist",
        "Skupiny ukladatelů s tímto kódem neexistuje."
    );
    
    public static Error ValidationNameIsRequired => Error.Validation(
        ComponentSlug + "validationNameIsRequired",
        "Jméno ukladatele není vyplněno."
    );
    
    public static Error CodeDoesNotExist => Error.NotFound(
        ComponentSlug + "codeDoesNotExist",
        "Ukladatel s tímto kódem neexistuje.");
    
    public static Error ValidationCodeDoesNotExist => Error.Validation(
        ComponentSlug + "validationCodeDoesNotExist",
        "Ukladatel s tímto kódem neexistuje.");
    
    public static Error ValidationCodeAlreadyExists => Error.Validation(
        ComponentSlug + "validationCodeAlreadyExists",
        "Ukladatel s tímto kódem již existuje."
    );
    
    public static Error ValidationCodeIsAssignedToUsers(string userNames) => Error.Validation(
        ComponentSlug + "validationCodeIsAssignedToUsers",
        $"Nemůžeme smazat ukladatele, protože je stále nastaven u těchto uživatelů: {userNames}."
    );
    
    public static Error ValidationDeliveryTemplateDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationDeliveryTemplateDoesNotExist",
        "Šablona dodacího listu s tímto kódem neexistuje."
    );
    
    public static Error ValidationLoadingTemplateDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationLoadingTemplateDoesNotExist",
        "Šablona nákladního listu s tímto kódem neexistuje."
    );
    
}