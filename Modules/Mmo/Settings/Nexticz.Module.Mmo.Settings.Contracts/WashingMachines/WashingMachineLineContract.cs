using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;

public record WashingMachineLineContract(
    [property: Required] string Code,
    [property: Required] bool IsActive,
    [property: Required] PrinterSettingsContract PrinterSettings);