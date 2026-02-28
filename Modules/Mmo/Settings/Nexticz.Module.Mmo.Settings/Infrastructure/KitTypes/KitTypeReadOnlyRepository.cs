using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.KitTypes;

internal class KitTypeReadOnlyRepository(ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IKitTypeReadOnlyRepository
{
    public async Task<KitType?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<KitType>(
                x => x.Code == code.ToUpperInvariant(), cancellationToken);
    }
}