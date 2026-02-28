using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments;


internal abstract class LoadingDocumentErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-documentManager-api-loadingDocumentService-";

    public static Error LoadingDocumentNotFound => Error.NotFound(
        ComponentSlug + "LoadingDocumentNotFound",
        "Nákladní list nebyl nalezen."
    );
    
    public static Error ValidationLoadingDocumentDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationLoadingDocumentDoesNotExist",
        "Nákladní list neexistuje."
    );
    
    public static Error ValidationDocumentTemplatesDontExist => Error.Validation(
        ComponentSlug + "ValidationDocumentTemplatesDontExist",
        "Šablony pro dokumenty neexistují."
    );
    
    public static Error ValidationThisLoadingDocumentDoesNotExist(string loadingDocumentCode) => Error.Validation(
        ComponentSlug + "ValidationThisLoadingDocumentDoesNotExist",
        $"Nákladní list neexistuje: {loadingDocumentCode}."
    );
    
    public static Error ValidationCurrentUserNameIsNotPresentInUsers => Error.Validation(
        ComponentSlug + "ValidationCurrentUserNameIsNotPresentInUsers",
        "Uživatelské jméno není v seznamu uživatelů pro tento modul."
    );
    
    public static Error ValidationNoRequestedDocumentsForDownload => Error.Validation(
        ComponentSlug + "ValidationNoRequestedDocumentsForDownload",
        "Nejsou označeny žádné dokumenty ke stažení."
    );
    
    public static Error ValidationNoRequestedDocumentsForDelete => Error.Validation(
        ComponentSlug + "ValidationNoRequestedDocumentsForDelete",
        "Nejsou označeny žádné dokumenty ke smazání."
    );
    
    public static Error ValidationCurrentUserCannotManageLoadingDocument => Error.Validation(
        ComponentSlug + "ValidationCurrentUserCannotManageLoadingDocument",
        "Uživatel nemá práva spravovat nákladní list."
    );
    
    public static Error ValidationDeliveryDocumentDoesNotExistInLoadingDocument(string loadingDocumentCode, string deliveryDocumentCode) => Error.Validation(
        ComponentSlug + "ValidationDeliveryDocumentDoesNotExistInLoadingDocument",
        $"Dodací list neexistuje v nakládkovém listu. Nakládkový kód: '{loadingDocumentCode}', dodací kód: '{deliveryDocumentCode}'."
    );
    
    public static Error ValidationNoRecipientsForEmailAttempt => Error.Validation(
        ComponentSlug + "ValidationNoRecipientsForEmailAttempt",
        "Nejsou specifikováni žádní příjemci e-mailu."
    );
    
    public static Error ValidationFileNotFound => Error.Validation(
        ComponentSlug + "ValidationFileNotFound",
        "Soubor nebyl nalezen."
    );
    
    public static Error ValidationNoReasonForDeletion => Error.Validation(
        ComponentSlug + "ValidationNoReasonForDeletion",
        "Musíte zadat důvod pro smazání."
    );
}