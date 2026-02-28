using System.Text.Json.Serialization;

namespace Nexticz.Module.Mmo.Reporting.Contracts.DriedKits;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DriedKitDestinationContract
{
    CompletingSection,
    DryingSection
}