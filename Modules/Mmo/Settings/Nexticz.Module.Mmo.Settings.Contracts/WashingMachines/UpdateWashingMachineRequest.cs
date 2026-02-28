using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;

public record UpdateWashingMachineRequest(
    [property: Required] string Note,
    [property: Required] WashingMachineStatusContract Status,
    [property: Required] int Speed1,
    [property: Required] int Speed2,
    [property: Required] int Speed3,
    [property: Required] int MaxWaterTemperature,
    [property: Required] int MaxAirTemperature,
    [property: Required] int MinWidth,
    [property: Required] WashingMachineLineContract[] WashingMachineLines);