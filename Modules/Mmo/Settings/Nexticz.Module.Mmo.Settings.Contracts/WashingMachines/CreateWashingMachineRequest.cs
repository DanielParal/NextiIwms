using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;

public record CreateWashingMachineRequest(
    [property: Required] string Code,
    [property: Required] string Note,
    [property: Required] WashingMachineStatusContract Status,
    [property: Required] int Length,
    [property: Required] int MinWidth,
    [property: Required] int MaxWidth,
    [property: Required] int MaxHeight,
    [property: Required] int MaxWaterTemperature,
    [property: Required] int MaxAirTemperature,
    [property: Required] int NumberOfLines,
    [property: Required] int Speed1,
    [property: Required] int Speed2,
    [property: Required] int Speed3);