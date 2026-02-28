namespace Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

public class SentLoadingDocument(Guid loadingDocumentId, string loadingDocumentCode, string? licensePlate, string? driverName, decimal? totalWeight, int? totalAdrPoints, SentDeliveryDocument[] deliveryDocuments, bool shouldAlsoSendLoadingDocument)
{
    public Guid LoadingDocumentId { get; private set; } = loadingDocumentId;
    public string LoadingDocumentCode { get; private set; } = loadingDocumentCode;
    public string? LicensePlate { get; private set; } = licensePlate;
    public string? DriverName { get; private set; } = driverName;
    public decimal? TotalWeight { get; private set; } = totalWeight;
    public int? TotalAdrPoints { get; private set; } = totalAdrPoints;
    public SentDeliveryDocument[] DeliveryDocuments { get; private set; } = deliveryDocuments;
    public bool ShouldAlsoSendLoadingDocument { get; private set; } = shouldAlsoSendLoadingDocument;
    
}