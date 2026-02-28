using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Reporting.Contracts.Shifts;

public record WashingMachineDetailContract(
    [property: Required] string Code,
    [property: Required] string Name,
    [property: Required] bool IsHelpNeeded);