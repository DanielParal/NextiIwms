using System.Text.Json.Serialization;

namespace Nexticz.Module.Cuzk.Contracts.Imports;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ImportStatusContract
{
    ImportSucceeded,
    ImportedSucceededWithErrors,
    ImportFailed,
    Requested
}