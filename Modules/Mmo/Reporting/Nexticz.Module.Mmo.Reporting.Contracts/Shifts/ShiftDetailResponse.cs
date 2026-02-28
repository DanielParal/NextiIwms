using System.ComponentModel.DataAnnotations;
using Nexticz.Module.Mmo.Reporting.Contracts.LineItems;

namespace Nexticz.Module.Mmo.Reporting.Contracts.Shifts;

public record ShiftDetailResponse(
    [property: Required] WashingMachineDetailContract[] WashingMachines,
    [property: Required] WashingMachineLineDetailContract[] Lines,
    [property: Required] LineItemContract[] LineItems,
    [property: Required] LineItemContract[] LineItemsForReview,
    [property: Required] int ShiftStartHour,
    [property: Required] int ShiftEndHour,
    [property: Required] int ShiftDurationInHours,
    [property: Required] ShiftStatusContract ShiftStatus);