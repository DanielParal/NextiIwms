using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings;

internal class PackagingResponseFactory
{
    public static PackagingResponse Create(Packaging packaging)
    {
        var washingMachineSpeeds = packaging
            .WashingMachineSpeeds
            .Select(x => new WashingMachineSpeedContract(x.WashingMachineCode, (WashingMachineSpeedLevelContract)x.Speed))
            .ToArray();
        
        var dimensions = new DimensionsContract(
            packaging.Dimensions.Depth, 
            packaging.Dimensions.Width, 
            packaging.Dimensions.Height);
        
        return new PackagingResponse(
            packaging.Id,
            packaging.Code,
            packaging.PackagingTypeCode,
            packaging.DepositorCode, 
            packaging.PackagingCirculationCode,
            packaging.CustomerNumber, 
            packaging.Name, 
            packaging.MustBeWashed,
            dimensions, 
            packaging.Weight,
            washingMachineSpeeds);
    }
}