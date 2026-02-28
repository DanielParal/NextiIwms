using System.Text.Json.Serialization;

namespace Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WashingMachineStatusContract
{
    Working,
    Maintenance,
    Eliminated
}