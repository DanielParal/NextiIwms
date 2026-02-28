namespace Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;

public record ChangePrintCopiesCountRequest(string? DeliveryDocumentCode, int PrintCopiesCount);