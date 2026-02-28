using ErrorOr;

namespace Nexticz.Module.Mmo.Planning.Application.WashingCalculators;

public class WashingTimeCalculator
{
    /// <summary>
    /// Example:
    /// ---------
    /// PackagingWidth - 400mm
    /// PackagingCount - 32
    /// WashingMachineSpeed - 80mm/s
    /// SpaceBetweenPackaging (constant) - 100mm
    /// 
    /// 1. Firstly we calculate the total length of all packagings and spaces between them:
    /// (32*400)+(31*100) = 15900mm
    /// 2. After that we calculate standard time for kit - divide total length with speed:
    /// 15900/80 = 198,75s (3,31 minutes)
    /// 
    /// </summary>
    /// <param name="packagingWidth">Users put packaging on the width side on the washing machine. The value is in millimeters</param>
    /// <param name="packagingCount">Number of packagings which are on one kit.</param>
    /// <param name="washingMachineSpeed">The value is in millimeters/seconds.</param>
    /// <param name="spaceBetweenBoxes">Constant which is set in settings module.</param>
    /// <returns>
    ///     Returns optimal time in which the kit should be washed. This time is than compared with the actual time and overall efficiency is calculated.
    /// </returns>
    public static ErrorOr<TimeSpan> CalculateOptimalKitTime(
        decimal packagingWidth, 
        int packagingCount,
        int washingMachineSpeed,
        int spaceBetweenBoxes)
    {
        if (packagingWidth <= 0)
            return WashingCalculatorErrors.ValidationPackagingWidth;
        
        if (packagingCount <= 0)
            return WashingCalculatorErrors.ValidationPackagingCount;
        
        if (washingMachineSpeed <= 0)
            return WashingCalculatorErrors.ValidationWashingMachineSpeed;
        
        var totalWashingLength = (packagingCount * packagingWidth) + ((packagingCount - 1) * spaceBetweenBoxes);
        var kitStandardTime = totalWashingLength / washingMachineSpeed;
        var kitStandardTimeInSeconds = TimeSpan.FromSeconds(Math.Round((double)kitStandardTime));

        return kitStandardTimeInSeconds;
    }
}