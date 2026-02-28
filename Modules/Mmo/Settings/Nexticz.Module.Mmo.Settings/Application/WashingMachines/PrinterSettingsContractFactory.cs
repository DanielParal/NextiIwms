using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;

namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines;

internal static class PrinterSettingsContractFactory
{
    public static PrinterSettingsContract Create(PrinterSettings printerSettings)
    {
        return new PrinterSettingsContract(
            printerSettings.Ip, printerSettings.UserName, printerSettings.Password,
            printerSettings.CopyCount, (PrinterPageSizeContract?)printerSettings.PageSize);
    }
}