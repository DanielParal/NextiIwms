using System.Text.Json.Serialization;

namespace Nexticz.Module.Cuzk.Contracts.Imports;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ImportTypeContract
{
    MunicipalityCsv,
    AddressLocationCsv,
    AddressLocationZip
}