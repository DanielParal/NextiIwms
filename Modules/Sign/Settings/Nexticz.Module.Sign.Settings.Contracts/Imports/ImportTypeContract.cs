using System.Text.Json.Serialization;

namespace Nexticz.Module.Sign.Settings.Contracts.Imports;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ImportTypeContract
{
    PartnerXlsx,
    ReceiverXlsx,
    DeliveryMethodXlsx,
    DepositorGroupXlsx,
    DepositorXlsx
}