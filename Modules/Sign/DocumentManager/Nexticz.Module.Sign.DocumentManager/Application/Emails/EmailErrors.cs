using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.DocumentManager.Application.Emails;

internal abstract class EmailErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-documentManager-application-emailService-";
    
    public static Error ValidationCurrentUserNameIsNotPresentInUsers => Error.Validation(
        ComponentSlug + "ValidationCurrentUserNameIsNotPresentInUsers",
        "Uživatelské jméno není v seznamu uživatelů pro tento modul."
    );
    
    public static Error ValidationInvalidEmailAddress(string invalidEmail) => Error.Validation(
        ComponentSlug + "ValidationInvalidEmailAddress",
        $"Emailová adresa není validní. Adresa: '{invalidEmail}'."
    );
    
    public static Error ValidationLoadingDocumentDoesNotExists(string missingCode) => Error.Validation(
        ComponentSlug + "ValidationLoadingDocumentDoesNotExists",
        $"Nakládkový list není v databázi: '{missingCode}'."
    );
    
    public static Error ValidationLoadingDocumentIsNotFinished(string loadingDocumentCode) => Error.Validation(
        ComponentSlug + "ValidationLoadingDocumentIsNotFinished",
        $"Nakládkový list není ukončen: '{loadingDocumentCode}'."
    );
    
    public static Error ValidationUserCannotManageLoadingDocument(string loadingDocumentCode) => Error.Validation(
        ComponentSlug + "ValidationUserCannotManageLoadingDocument",
        $"Uživatel nemá práva na nakládkový list: '{loadingDocumentCode}'."
    );
    
    public static Error ValidationDeliveryDocumentDoNotExists(string loadingDocumentCode, string missingDeliveryDocumentCode) => Error.Validation(
        ComponentSlug + "ValidationDeliveryDocumentDoNotExists",
        $"Dodací list není v databázi. Nakládkový list: '{loadingDocumentCode}', dodací list: '{missingDeliveryDocumentCode}'."
    );
    
    public static Error ValidationDocumentIsNotFinished(string loadingDocumentCode, string deliveryDocumentCode) => Error.Validation(
        ComponentSlug + "ValidationDocumentIsNotFinished",
        $"Dodací list není ukončen. Kód nakládkového listu: '{loadingDocumentCode}', kód dodacího listu: '{deliveryDocumentCode}'."
    );
}