using System.Text.Json.Serialization;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SpeedLevelContract
{
    NotSet,
    Speed1,
    Speed2,
    Speed3
}