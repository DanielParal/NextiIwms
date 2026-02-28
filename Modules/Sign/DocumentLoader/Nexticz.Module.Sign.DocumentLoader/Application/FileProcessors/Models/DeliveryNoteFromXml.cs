namespace Nexticz.Module.Sign.DocumentLoader.Application.FileProcessors.Models;

internal class DeliveryNoteFromXml(DeliveryNoteXmlDoc deliveryNoteXmlDoc)
{
    public string Code { get; private set; } = deliveryNoteXmlDoc.DeliveryNoteXml.Code;
    public string LoadingListCode { get; private set; } = deliveryNoteXmlDoc.DeliveryNoteXml.LoadingListCode;
    public string DepositorCode { get; private set; } = deliveryNoteXmlDoc.DeliveryNoteXml.DepositorCode;
    public string PartnerCode { get; private set; } = deliveryNoteXmlDoc.DeliveryNoteXml.PartnerCode;
    public string PartnerNameShort { get; private set; } = deliveryNoteXmlDoc.DeliveryNoteXml.PartnerNameShort;
    public string DeliveryMethodCode { get; private set; } = deliveryNoteXmlDoc.DeliveryNoteXml.DeliveryMethodCode;
    public DateTime IssueDate { get; private set; } = deliveryNoteXmlDoc.DeliveryNoteXml.IssueDate;
    public string WarehouseCode { get; private set; } = deliveryNoteXmlDoc.DeliveryNoteXml.WarehouseCode;
    public string PartnersOrderNumber { get; private set; } = deliveryNoteXmlDoc.DeliveryNoteXml.PartnersOrderNumber;
    public string OperationalUnitCode { get; private set; } = deliveryNoteXmlDoc.DeliveryNoteXml.OperationalUnitCode;
    public string OperationalUnitName { get; private set; } = deliveryNoteXmlDoc.DeliveryNoteXml.OperationalUnitName;
    public string? RznoCode { get; private set; } = deliveryNoteXmlDoc.DeliveryNoteXml.CargoWindowReservationCode;
    public string? CombinedRznoCode { get; private set; } = deliveryNoteXmlDoc.DeliveryNoteXml.CombinedCargoWindowReservationCode;
    public decimal? WeightCalculated { get; private set; } = deliveryNoteXmlDoc.DeliveryNoteXml.WeightCalculated;
    public int? AdrPoints { get; private set; } = deliveryNoteXmlDoc.DeliveryNoteXml.AdrPoints;
}