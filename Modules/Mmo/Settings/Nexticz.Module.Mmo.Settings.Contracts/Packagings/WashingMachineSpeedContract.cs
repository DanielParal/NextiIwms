using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Packagings;

public record WashingMachineSpeedContract(
    [property: Required] string WashingMachineCode, 
    [property: Required] WashingMachineSpeedLevelContract Speed);