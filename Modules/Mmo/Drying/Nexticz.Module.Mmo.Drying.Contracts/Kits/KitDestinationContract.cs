using System.Text.Json.Serialization;

namespace Nexticz.Module.Mmo.Drying.Contracts.Kits;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum KitDestinationContract
{
    CompletingSection,
    DryingSection
}