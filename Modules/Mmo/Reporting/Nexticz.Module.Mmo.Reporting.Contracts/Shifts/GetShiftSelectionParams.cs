using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Reporting.Contracts.Shifts;

public record GetShiftSelectionParams(
    [property: Required] DateOnly SelectedDate);