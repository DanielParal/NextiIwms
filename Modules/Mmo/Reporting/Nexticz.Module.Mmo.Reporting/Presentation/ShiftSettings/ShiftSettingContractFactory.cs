using Nexticz.Module.Mmo.Reporting.Contracts.ShiftSettings;
using Nexticz.Module.Mmo.Reporting.Application.ShiftSettings;

namespace Nexticz.Module.Mmo.Reporting.Presentation.ShiftSettings;

internal static class ShiftSettingContractFactory
{
    public static ShiftSettingContract Create(ShiftSetting shiftSetting)
    {
        return new ShiftSettingContract(
            shiftSetting.Name, 
            shiftSetting.IsActive,
            shiftSetting.DailyOrder, 
            shiftSetting.Schedule.StartTimeOnly,
            shiftSetting.Schedule.EndTimeOnly);
    }
}