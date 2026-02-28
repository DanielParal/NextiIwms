using Nexticz.Module.Mmo.Settings.Contracts.KitTypes;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Presentation.KitTypes;

internal class KitTypeResponseFactory
{
    public static KitTypeResponse Create(KitType kitType)
    {
        return new KitTypeResponse(kitType.Id, kitType.Code, kitType.Name);
    }
}