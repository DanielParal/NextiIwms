namespace Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;

public record PrinterSettingsContract(
    string? Ip,
    string? UserName,
    string? Password,
    int? CopyCount,
    PrinterPageSizeContract? PageSize);