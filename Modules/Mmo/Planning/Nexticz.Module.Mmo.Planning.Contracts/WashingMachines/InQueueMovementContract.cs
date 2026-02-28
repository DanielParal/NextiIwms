using System.Text.Json.Serialization;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InQueueMovementContract
{
    FirstInQueue,
    OneUp,
    OneDown
}