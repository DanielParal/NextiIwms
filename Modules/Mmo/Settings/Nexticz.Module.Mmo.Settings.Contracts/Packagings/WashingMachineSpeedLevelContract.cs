using System.Text.Json.Serialization;

namespace Nexticz.Module.Mmo.Settings.Contracts.Packagings;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WashingMachineSpeedLevelContract
{
    NotSet,
    Speed1,
    Speed2,
    Speed3
}