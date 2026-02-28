using System.Text.Json.Serialization;

namespace Nexticz.Module.Mmo.Settings.Contracts.Exports;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExportTypeContract
{
    PackagingXlsx,
    KitXlsx
}