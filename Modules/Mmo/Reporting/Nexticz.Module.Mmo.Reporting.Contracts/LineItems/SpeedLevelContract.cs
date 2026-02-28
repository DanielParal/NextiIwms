using System.Text.Json.Serialization;

namespace Nexticz.Module.Mmo.Reporting.Contracts.LineItems;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SpeedLevelContract
{
    NotSet,
    Speed1,
    Speed2,
    Speed3
}