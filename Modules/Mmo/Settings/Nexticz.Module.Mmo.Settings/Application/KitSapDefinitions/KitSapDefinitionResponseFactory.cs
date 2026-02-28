using Nexticz.Module.Mmo.Settings.Contracts.KitSapDefinitions;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity;

namespace Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions;

internal class KitSapDefinitionResponseFactory
{
    public static KitSapDefinitionResponse Create(KitSapDefinition kitSapDefinition)
    {
        return new KitSapDefinitionResponse(
            kitSapDefinition.Id, kitSapDefinition.Code, kitSapDefinition.Name);
    }
}