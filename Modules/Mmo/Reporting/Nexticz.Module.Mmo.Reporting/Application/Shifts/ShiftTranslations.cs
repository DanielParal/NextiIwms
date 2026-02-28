using Nexticz.Lib.Shared.Translations;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts;

internal static class ShiftTranslations
{
    public static readonly Translation WashingMachineName = new($"MMO-reporting-{nameof(WashingMachineName)}", "Myčka");
    public static readonly Translation LineName = new($"MMO-reporting-{nameof(LineName)}", "Dráha");
    public static readonly Translation SpeedLineName = new($"MMO-reporting-{nameof(SpeedLineName)}", "Rychlost");
}