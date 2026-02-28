using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Reporting.Contracts.Shifts;

public record ShiftSelectionResponse(
    [property: Required] Guid Id,
    [property: Required] string Name,
    [property: Required] DateOnly ShiftDate,
    [property: Required] ShiftStatusContract Status,
    [property: Required] ShiftSelectionCategoryContract Category);