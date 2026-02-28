namespace Nexticz.Module.Sign.DocumentLoader.Application.FileProcessors.Models;

internal class LoadingListFromXml(LoadingListXmlDoc loadingListXmlDoc)
{
    
    public string Code { get; private set; } = loadingListXmlDoc.LoadingListXml.Code;
    public int? GateNumber { get; private set; } = loadingListXmlDoc.LoadingListXml.GateNumber;
    public string DeliveryMethodCode { get; private set; } = loadingListXmlDoc.LoadingListXml.DeliveryMethodCode;
    public string? LicensePlate { get; private set; } = loadingListXmlDoc.LoadingListXml.LicensePlate;
    public string? DriverName { get; private set; } = loadingListXmlDoc.LoadingListXml.DriverName;
    public decimal? Weight { get; private set; } = loadingListXmlDoc.LoadingListXml.Weight;
    public int? AdrPoints { get; private set; } = loadingListXmlDoc.LoadingListXml.AdrPoints;
    public string? LoadingLocation { get; private set; } = loadingListXmlDoc.LoadingListXml.FirstLocationOfShippingUnit;
    public string LoadingInWmsFinishedBy { get; private set; } = loadingListXmlDoc.LoadingListXml.ModifiedBy;
    public DateTime LoadingInWmsFinishedAt { get; private set; } = loadingListXmlDoc.LoadingListXml.Modified;
    public int TotalDeliveryNotes { get; private set; } = loadingListXmlDoc.LoadingListXml.DeliveryNotes.Count;
    public List<DeliveryNoteFromXml> DeliveryNotes { get; private set; } = [];
}