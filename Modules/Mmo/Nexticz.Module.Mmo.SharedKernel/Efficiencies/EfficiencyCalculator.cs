namespace Nexticz.Module.Mmo.SharedKernel.Efficiencies;

public static class EfficiencyCalculator
{
    public static double Calculate(DateTimeOffset startDate, DateTimeOffset endDate, TimeSpan optimalKitDuration)
    {
        if (startDate > endDate)
        {
            throw new ArgumentException("The 'start' date must be earlier than the 'end' date.");
        }

        var actualDuration = endDate - startDate;
        var efficiency = (optimalKitDuration.TotalMilliseconds / actualDuration.TotalMilliseconds) * 100;
        
        return EfficiencyFormatter.Round(efficiency);
    }
    
    public static double Calculate(TimeSpan realTimeKitDuration, TimeSpan optimalKitDuration)
    {
        var efficiency = (optimalKitDuration.TotalMilliseconds / realTimeKitDuration.TotalMilliseconds) * 100;
        
        return EfficiencyFormatter.Round(efficiency);
    }
}