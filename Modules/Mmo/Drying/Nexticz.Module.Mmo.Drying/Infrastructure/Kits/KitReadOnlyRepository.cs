using Nexticz.Module.Mmo.Drying.Application.Interfaces;
using Nexticz.Module.Mmo.Drying.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Drying.Infrastructure.Kits;

internal class KitReadOnlyRepository(IDryingReadOnlyEventStoreRepository dryingReadOnlyEventStoreRepository) : IKitReadOnlyRepository
{
    public async Task<Kit?> GetKitByCompletedKitsCountAsync(int completedKitsCount, CancellationToken cancellationToken)
    {
        return await dryingReadOnlyEventStoreRepository.GetFirstByConditionAsync<Kit>(
            x => x.GlobalKitsCount == completedKitsCount, cancellationToken);
    }
}