using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.KitSapDefinitions;

internal class KitSapDefinitionReadOnlyRepository(ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IKitSapDefinitionReadOnlyRepository
{
    public async Task<KitSapDefinition?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<KitSapDefinition>(
                x => x.Code == code.ToUpperInvariant(), cancellationToken);
    }
}