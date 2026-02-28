using System.Text.Json.Serialization;

namespace Nexticz.Module.Mmo.Washing.Contracts.Batches;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PrintingStatusContract
{
    Success,
    Failed
}