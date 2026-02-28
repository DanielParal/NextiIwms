using System.Text.Json.Serialization;

namespace Nexticz.Module.Mmo.Reporting.Contracts.LineItems;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LineItemTypeContract
{
    Adjustment,
    Downtime,
    Shutdown,
    Break,
    Kit,
    Speed
}