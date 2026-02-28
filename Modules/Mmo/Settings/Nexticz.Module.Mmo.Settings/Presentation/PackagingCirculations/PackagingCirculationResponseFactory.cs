using Nexticz.Module.Mmo.Settings.Contracts.PackagingCirculations;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;

namespace Nexticz.Module.Mmo.Settings.Presentation.PackagingCirculations;

internal class PackagingCirculationResponseFactory
{
    public static PackagingCirculationResponse Create(PackagingCirculation packagingCirculation)
    {
        return new PackagingCirculationResponse(packagingCirculation.Id, packagingCirculation.Code, packagingCirculation.Name);
    }
}