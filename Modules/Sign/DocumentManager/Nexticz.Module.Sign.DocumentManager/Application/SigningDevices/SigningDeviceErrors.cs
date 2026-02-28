using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices;


internal abstract class SigningDeviceErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-documentManager-api-signingDeviceService-";
    
    public static Error ValidationSigningDeviceAlreadyExist => Error.Validation(
        ComponentSlug + "ValidationSigningDeviceAlreadyExist",
        "Podpisové zařízení již existuje."
    );
    
    public static Error SigningDeviceNotFound => Error.NotFound(
        ComponentSlug + "SigningDeviceNotFound",
        "Podpisové zařízení nenalezeno."
    );
    
    public static Error ValidationSigningDeviceCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationSigningDeviceCodeIsRequired",
        "Kód podpisového zařízení není vyplněn."
    );
    
    public static Error ValidationDriverNameIsRequired => Error.Validation(
        ComponentSlug + "ValidationDriverNameIsRequired",
        "Jméno řidiče není vyplněno."
    );
    
    public static Error ValidationLicensePlateIsRequired => Error.Validation(
        ComponentSlug + "ValidationLicensePlateIsRequired",
        "SPZ není vyplněna."
    );
    
    public static Error ValidationDocumentCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationDocumentCodeIsRequired",
        "Kód dokumentu je povinné pole."
    );
    
    public static Error ValidationCurrentUserCannotManageSigningDevice => Error.Validation(
        ComponentSlug + "ValidationCurrentUserCannotManageSigningDevice",
        "Uživatel nemá práva spravovat podpisové zařízení."
    );
    
    public static Error ValidationDeviceBusyTimeoutToAcquireLockForDevice => Error.Validation(
        ComponentSlug + "ValidationDeviceBusyTimeoutToAcquireLockForDevice",
        "Jiný uživatel posílá dokumenty na to samé zařízení. Zkuste to znovu za malou chvíli."
    );

    
    public static Error ValidationSigningDeviceWithCodeDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationSigningDeviceWithCodeDoesNotExist",
        "Podpisové zařízení s tímto kódem neexistuje."
    );
    
    public static Error ValidationCurrentUserNameIsNotPresentInUsers => Error.Validation(
        ComponentSlug + "ValidationCurrentUserNameIsNotPresentInUsers",
        "Uživatelské jméno není v seznamu uživatelů pro tento modul."
    );
    
    public static Error ValidationUserWhoSentDocumentsToSigningDeviceDoesNotHaveSignatureFile => Error.Validation(
        ComponentSlug + "ValidationUserWhoSentDocumentsToSigningDeviceDoesNotHaveSignatureFile",
        "Uživatelské, který poslal dokumenty na tablet nemá nastaven podpisový soubor."
    );
    
    public static Error ValidationThereAreAlreadyFilesOnTheSigningDevice => Error.Validation(
        ComponentSlug + "ValidationThereAreAlreadyFilesOnTheSigningDevice",
        "Na podpisovém zařízení jsou jiné dokumenty k podpisu."
    );
    
    public static Error ValidationThereAreNoFilesOnTheSigningDevice => Error.Validation(
        ComponentSlug + "ValidationThereAreNoFilesOnTheSigningDevice",
        "Na podpisovém zařízení nejsou žádné dokumenty k podpisu."
    );
    
    public static Error ValidationThereIsNoUserWhoSentTheDocuments => Error.Validation(
        ComponentSlug + "ValidationThereIsNoUserWhoSentTheDocuments",
        "Chybí jméno uživatele, který poslal dokumenty k podpisu."
    );
    
    public static Error ValidationUnexpectedError(string correlationId) => Error.Validation(
        ComponentSlug + "ValidationUnexpectedError",
        $"Nastala neočekávaná chyba. Pro více informací kontaktujte podporu s tímto kódem: '{correlationId}'."
    );
    
    public static Error ValidationSomeLoadingDocumentsDoNotExists(string missingCodes) => Error.Validation(
        ComponentSlug + "ValidationSomeLoadingDocumentsDoNotExists",
        $"Některé nakládkové listy nejsou v databázi: '{missingCodes}'."
    );
    
    public static Error ValidationCurrentUserCannotManageLoadingDocument => Error.Validation(
        ComponentSlug + "ValidationCurrentUserCannotManageLoadingDocument",
        "Uživatel nemá práva spravovat nákladní list."
    );
    
    public static Error ValidationNoDocumentsFilledIn => Error.Validation(
        ComponentSlug + "ValidationNoDocumentsFilledIn",
        "Nevybrali jste žádné dokumenty k podpisu."
    );
    
    public static Error ValidationYouMustFillInLoadingDocumentCodes => Error.Validation(
        ComponentSlug + "ValidationYouMustFillInLoadingDocumentCodes",
        "U všech dokumentů k podpisu musí být poslán kód nakládkového listu."
    );
    
    public static Error ValidationAtLeastOneDeliveryDocumentMustBeFilledInWhenLoadingDocumentIsNotSent => Error.Validation(
        ComponentSlug + "ValidationAtLeastOneDeliveryDocumentMustBeFilledInWhenLoadingDocumentIsNotSent",
        "Musíte vybrat alespon jeden dokument k podpisu."
    );
    
    public static Error ValidationNoDocumentsOnTheDevice => Error.Validation(
        ComponentSlug + "ValidationNoDocumentsOnTheDevice",
        "Na podpisovém zařízení nejsou žádné dokumenty k podpisu."
    );
    
    public static Error ValidationDocumentCodeIsNotOnTheDevice => Error.Validation(
        ComponentSlug + "ValidationDocumentCodeIsNotOnTheDevice",
        "Dokument se nevyskytuje na podpisovém zařízení."
    );
    
    public static Error ValidationFileIsNotFound => Error.Validation(
        ComponentSlug + "ValidationFileIsNotFound",
        "Soubor nebyl nalezen."
    );
    
    public static Error ValidationCurrentUserDoesNotHaveSignatureFile => Error.Validation(
        ComponentSlug + "ValidationCurrentUserDoesNotHaveSignatureFile",
        "Uživatel musí mít nastaven soubor s podpisem v nastavení."
    );
    
    public static Error ValidationCurrentUserDoesNotHaveFullName => Error.Validation(
        ComponentSlug + "ValidationCurrentUserDoesNotHaveFullName",
        "Uživatel musí mít nastaveno jméno v nastavení."
    );
}