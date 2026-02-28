namespace Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

internal record EmailInitiatorProperties(
    string LoadingDocumentCode,
    string? DeliveryDocumentCode);