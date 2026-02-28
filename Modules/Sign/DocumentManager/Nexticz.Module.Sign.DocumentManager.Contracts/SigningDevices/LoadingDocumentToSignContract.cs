using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;

public record LoadingDocumentToSignContract(
    [property: Required] string LoadingDocumentCode,
    [property: Required] DeliveryDocumentToSignContract[] DeliveryDocuments,
    [property: Required] bool ShouldAlsoSignLoadingDocument);