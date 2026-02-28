using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;

public record DownloadDocumentJobContract(
    [property: Required] string LoadingDocumentCode,
    string? DeliveryDocumentCode);