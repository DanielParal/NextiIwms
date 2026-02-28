
namespace Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;

public class WashingMachineLine(string code, bool isActive, PrinterSettings printerSettings)
{
    public string Code { get; private set; } = code;
    public bool IsActive { get; private set; } = isActive;
    public PrinterSettings PrinterSettings { get; private set; } = printerSettings;
}