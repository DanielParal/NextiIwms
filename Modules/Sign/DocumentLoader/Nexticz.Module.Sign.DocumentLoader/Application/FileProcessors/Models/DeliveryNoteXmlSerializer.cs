
using System.Xml.Serialization;

namespace Nexticz.Module.Sign.DocumentLoader.Application.FileProcessors.Models;

[XmlRoot("Doc")]
public class DeliveryNoteXmlDoc
{
    [XmlElement("DeliveryNote")]
    public DeliveryNoteXml DeliveryNoteXml { get; set; }
}

public class DeliveryNoteXml
{
    [XmlElement("LoadingDocument")]
    public string LoadingListCode { get; set; }

    [XmlElement("Document")]
    public string Code { get; set; }

    [XmlElement("DepositorCode")]
    public string DepositorCode { get; set; }

    [XmlElement("PartnerCode")]
    public string PartnerCode { get; set; }

    [XmlElement("PartnerNameShort")]
    public string PartnerNameShort { get; set; }

    [XmlElement("DeliveryMethodCode")]
    public string DeliveryMethodCode { get; set; }

    [XmlElement("IssueDate")]
    public DateTime IssueDate { get; set; }

    [XmlElement("WarehouseCode")]
    public string WarehouseCode { get; set; }

    [XmlElement("PartnersOrderNumber")]
    public string PartnersOrderNumber { get; set; }

    [XmlElement("OperationalUnitCode")]
    public string OperationalUnitCode { get; set; }

    [XmlElement("OperationalUnitName")]
    public string OperationalUnitName { get; set; }

    [XmlElement("CargoWindowReservationCode")]
    public string? CargoWindowReservationCode { get; set; }

    [XmlElement("CombinedCargoWindowReservationCode")]
    public string? CombinedCargoWindowReservationCode { get; set; }

    [XmlElement("Created")]
    public DateTime Created { get; set; }

    [XmlElement("WeightCalculated")]
    public decimal? WeightCalculated { get; set; }

    [XmlElement("AdrPoints")]
    public int? AdrPoints { get; set; }
}