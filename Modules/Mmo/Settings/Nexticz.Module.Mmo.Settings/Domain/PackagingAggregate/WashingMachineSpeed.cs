namespace Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;

public class WashingMachineSpeed (string washingMachineCode, WashingMachineSpeedLevel speed)
{
    public string WashingMachineCode { get; private set; } = washingMachineCode.ToUpperInvariant();
    public WashingMachineSpeedLevel Speed { get; private set; } = speed;
}