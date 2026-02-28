using Nexticz.Module.Mmo.Reporting.Application.Shifts;

namespace Nexticz.Module.Mmo.Reporting.Presentation.Shifts;

internal static class WashingMachineNameGenerator
{
    public static string GenerateWashingMachineName(int index)
    {
        return $"{ShiftTranslations.WashingMachineName.TranslationValue} {index}";
    }
    
    public static string GenerateWashingMachineLineName(int index)
    {
        return $"{ShiftTranslations.LineName.TranslationValue} {index}";
    }
    
    public static string GenerateWashingMachineLineSpeedName()
    {
        return $"{ShiftTranslations.SpeedLineName.TranslationValue}";
    }
}