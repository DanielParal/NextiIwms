using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

internal abstract class LoadingDocumentDomainErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-documentManager-domain-loadingDocument-";
    
    public static Error ValidationLoadingDocumentAlreadySentToSigningDevice(string loadingDocumentCode) => Error.Validation(
        ComponentSlug + "ValidationLoadingDocumentAlreadySentToSigningDevice",
        $"Nákladní list: {loadingDocumentCode} je již poslán na podpisové zařízení."
    );
    
    public static Error ValidationLoadingDocumentIsAlreadyFinished(string loadingDocumentCode) => Error.Validation(
        ComponentSlug + "ValidationLoadingDocumentIsAlreadyFinished",
        $"Nákladní list: {loadingDocumentCode} je již ukončen."
    );
    
    public static Error ValidationLoadingDocumentIsNotFinished(string loadingDocumentCode) => Error.Validation(
        ComponentSlug + "ValidationLoadingDocumentIsNotFinished",
        $"Nákladní list: {loadingDocumentCode} není ukončen."
    );
    
    public static Error ValidationLoadingDocumentIsNotSentToSigningDevice(string loadingDocumentCode) => Error.Validation(
        ComponentSlug + "ValidationLoadingDocumentIsNotSentToSigningDevice",
        $"Nákladní list: {loadingDocumentCode} není na žádném zařízení."
    );
    
    public static Error ValidationDeliveryDocumentIsAlreadyFinished(string loadingDocumentCode, string deliveryDocumentCode) => Error.Validation(
        ComponentSlug + "ValidationDeliveryDocumentIsAlreadyFinished",
        $"Dodací list: {deliveryDocumentCode} obsažen v nákladním listu: {loadingDocumentCode} je již ukončen."
    );
    
    public static Error ValidationDeliveryDocumentIsNotFinished(string loadingDocumentCode, string deliveryDocumentCode) => Error.Validation(
        ComponentSlug + "ValidationDeliveryDocumentIsNotSigned",
        $"Dodací list: {deliveryDocumentCode} obsažen v nákladním listu: {loadingDocumentCode} není ukončen."
    );
    
    public static Error ValidationDeliveryDocumentDoesNotExistInLoadingDocument(string loadingDocumentCode, string deliveryDocumentCode) => Error.Validation(
        ComponentSlug + "ValidationDeliveryDocumentDoesNotExistInLoadingDocument",
        $"Nákladní list: {loadingDocumentCode} neobsahuje nákladní list: {deliveryDocumentCode}."
    );
    
    public static Error ValidationSomeDeliveryDocumentsDoNotExists(string missingCodes) => Error.Validation(
        ComponentSlug + "ValidationSomeDeliveryDocumentsDoNotExists",
        $"Některé dodací listy nejsou v databázi: '{missingCodes}'."
    );
    
    public static Error ValidationDeliveryDocumentAlreadySentToSigningDevice(string loadingDocumentCode, string deliveryDocumentCode) => Error.Validation(
        ComponentSlug + "ValidationDeliveryDocumentAlreadySentToSigningDevice",
        $"Dodací list je již poslán na podpisové zařízení. Nákladní list: {loadingDocumentCode}, dodací list: {deliveryDocumentCode}."
    );
    
    public static Error ValidationDeliveryDocumentIsNotSentToSigningDevice(string loadingDocumentCode, string deliveryDocumentCode) => Error.Validation(
        ComponentSlug + "ValidationDeliveryDocumentIsNotSentToSigningDevice",
        $"Dodací list niní poslán na podpisové zařízení. Nákladní list: {loadingDocumentCode}, dodací list: {deliveryDocumentCode}."
    );
    
    
}