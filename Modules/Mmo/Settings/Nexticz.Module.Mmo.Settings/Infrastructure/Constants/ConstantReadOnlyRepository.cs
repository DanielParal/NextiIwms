using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Constants;

internal class ConstantReadOnlyRepository(ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IConstantReadOnlyRepository
{
    public async Task<Constant?> GetByKeyAsync(string key, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<Constant>(
                x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase), cancellationToken);
    }
}