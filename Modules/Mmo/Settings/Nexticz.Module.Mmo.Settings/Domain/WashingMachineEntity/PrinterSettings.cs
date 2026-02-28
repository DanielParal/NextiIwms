namespace Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;

public record PrinterSettings(string? Ip, string? UserName, string? Password, int? CopyCount, PrinterPageSize? PageSize);