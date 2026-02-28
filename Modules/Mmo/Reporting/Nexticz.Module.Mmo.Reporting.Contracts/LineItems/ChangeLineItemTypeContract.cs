using System.Text.Json.Serialization;

namespace Nexticz.Module.Mmo.Reporting.Contracts.LineItems;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChangeLineItemTypeContract
{
    Adjustment,
    Downtime,
    Shutdown,
    Break
}