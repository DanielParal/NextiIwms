using System.Text.Json.Serialization;

namespace Nexticz.Module.Mmo.Reporting.Contracts.Shifts;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ShiftSelectionCategoryContract
{
    Today,
    Other
}