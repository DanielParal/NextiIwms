namespace Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate;

public class DocumentPrintJob(string loadingDocumentCode, string? deliveryDocumentCode, int copiesCount)
{
    public string LoadingDocumentCode { get; private set; } = loadingDocumentCode;
    public string? DeliveryDocumentCode { get; private set; } = deliveryDocumentCode;
    public int CopiesCount { get; private set; } = copiesCount;
}