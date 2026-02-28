using System.Text.Json.Serialization;

namespace Nexticz.Module.Sign.Settings.Contracts.Exports;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExportTypeContract
{
    PartnerXlsx,
    ReceiverXlsx,
    DeliveryMethodXlsx,
    DepositorGroupXlsx,
    DepositorXlsx
}