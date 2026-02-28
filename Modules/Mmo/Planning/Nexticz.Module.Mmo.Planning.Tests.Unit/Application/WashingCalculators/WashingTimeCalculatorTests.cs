using Nexticz.Module.Mmo.Planning.Application.WashingCalculators;
using Shouldly;

namespace Nexticz.Module.Mmo.Planning.Tests.Unit.Application.WashingCalculators;

public class WashingTimeCalculatorTests
{
    
    [Theory]
    [InlineData(260, 30, 83, 100, "00:02:09")]
    [InlineData(200, 50, 100, 100,  "00:02:29")]
    [InlineData(150, 20, 75, 100,  "00:01:05")]
    public void CalculateOptimalKitTime_ShouldReturnExpectedTime_WhenInputsAreValid(
        decimal packagingWidth, int packagingCount, int washingMachineSpeed, int spaceBetweenBoxes, string expectedResult)
    {
        var result = WashingTimeCalculator.CalculateOptimalKitTime(packagingWidth, packagingCount, washingMachineSpeed, spaceBetweenBoxes);
        result.Value.ShouldBe(TimeSpan.Parse(expectedResult));
    }

    [Theory]
    [InlineData(-1, 30, 83, 100)] // Negative packagingWidth
    [InlineData(260, -5, 100, 100)] // Negative packagingCount
    [InlineData(150, 20, -1, 100)] // Negative washingMachineSpeed
    public void CalculateOptimalKitTime_ShouldReturnError_WhenInputsAreInvalid(
        decimal packagingWidth, int packagingCount, int washingMachineSpeed, int spaceBetweenBoxes)
    {
        var result = WashingTimeCalculator.CalculateOptimalKitTime(packagingWidth, packagingCount, washingMachineSpeed, spaceBetweenBoxes);
        result.IsError.ShouldBeTrue();
    }
}