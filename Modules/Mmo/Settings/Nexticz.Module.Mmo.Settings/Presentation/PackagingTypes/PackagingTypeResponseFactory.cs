using Nexticz.Module.Mmo.Settings.Contracts.PackagingTypes;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Presentation.PackagingTypes;

internal class PackagingTypeResponseFactory
{
    public static PackagingTypeResponse Create(PackagingType packagingType)
    {
        return new PackagingTypeResponse(packagingType.Id, packagingType.Code, packagingType.Name);
    }
}