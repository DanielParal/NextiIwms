using System.Xml.Serialization;

namespace Nexticz.Module.Sign.DocumentLoader.Application.FileProcessors.Models;

[XmlRoot("Doc")]
public class LoadingListXmlDoc
{
    [XmlElement("LoadingList")]
    public LoadingListXml LoadingListXml { get; set; }
}

public class LoadingListXml
{
    [XmlElement("Document")]
    public string Code { get; set; }

    [XmlElement("GateNumber")]
    public int? GateNumber { get; set; }

    [XmlElement("DeliveryMethodCode")]
    public string DeliveryMethodCode { get; set; }

    [XmlElement("LicensePlate")]
    public string? LicensePlate { get; set; }

    [XmlElement("DriverName")]
    public string? DriverName { get; set; }

    [XmlElement("Weight")]
    public decimal? Weight { get; set; }

    [XmlElement("Modified")]
    public DateTime Modified { get; set; }

    [XmlElement("ModifiedBy")]
    public string ModifiedBy { get; set; }

    [XmlElement("AdrPoints")]
    public int? AdrPoints { get; set; }

    [XmlElement("FirstLocationOfShippingUnit")]
    public string? FirstLocationOfShippingUnit { get; set; }

    [XmlArray("DeliveryNotes")]
    [XmlArrayItem("DeliveryNote")]
    public List<DeliveryNoteName> DeliveryNotes { get; set; }
}

public class DeliveryNoteName
{
    [XmlAttribute("Document")]
    public string Name { get; set; }
}