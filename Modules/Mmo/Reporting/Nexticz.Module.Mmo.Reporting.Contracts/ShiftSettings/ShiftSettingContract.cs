using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Reporting.Contracts.ShiftSettings;

public record ShiftSettingContract(
    [property: Required] string Name,
    [property: Required] bool IsActive,
    [property: Required] int DailyOrder,
    [property: Required] TimeOnly StartTime,
    [property: Required] TimeOnly EndTime
    );