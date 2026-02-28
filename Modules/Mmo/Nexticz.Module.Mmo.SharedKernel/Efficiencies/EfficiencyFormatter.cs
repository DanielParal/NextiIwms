namespace Nexticz.Module.Mmo.SharedKernel.Efficiencies;

public static class EfficiencyFormatter
{
    public static double Round(double value)
    {
        return Math.Round(value, 2);
    }
    
    public static string ZeroToPercentageString => "0.00%";
    
    public static string EfficiencyToPercentageString(double efficiencyPercentage)
    {
        return $"{efficiencyPercentage:F2}%";
    }
}