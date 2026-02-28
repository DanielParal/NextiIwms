namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods.ImportExport;

internal class DeliveryMethodItem(
    string code, string name, 
    int loadingDocumentPrintCopiesCount, int deliveryDocumentPrintCopiesCount,
    int rowNumber)
{
    public string Code { get; } = code.ToUpperInvariant();
    public string Name { get; } = name;
    public int LoadingDocumentPrintCopiesCount { get; } = loadingDocumentPrintCopiesCount;
    public int DeliveryDocumentPrintCopiesCount { get; } = deliveryDocumentPrintCopiesCount;
    public int RowNumber { get; } = rowNumber;
}