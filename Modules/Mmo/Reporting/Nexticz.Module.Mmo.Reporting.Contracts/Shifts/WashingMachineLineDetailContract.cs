using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Reporting.Contracts.Shifts;

public record WashingMachineLineDetailContract(
    [property: Required] string Code,
    [property: Required] string Name);