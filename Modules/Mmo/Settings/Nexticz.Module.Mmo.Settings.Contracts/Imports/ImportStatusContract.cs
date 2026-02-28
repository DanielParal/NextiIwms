using System.Text.Json.Serialization;

namespace Nexticz.Module.Mmo.Settings.Contracts.Imports;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ImportStatusContract
{
    Success,
    PartialSuccessWithErrors,
    Failure,
    Requested
}