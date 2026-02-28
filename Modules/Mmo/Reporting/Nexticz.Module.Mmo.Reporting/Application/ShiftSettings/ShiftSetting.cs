namespace Nexticz.Module.Mmo.Reporting.Application.ShiftSettings;

internal class ShiftSetting
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public int DailyOrder { get; set; }
    public ShiftSettingSchedule Schedule { get; set; }
}