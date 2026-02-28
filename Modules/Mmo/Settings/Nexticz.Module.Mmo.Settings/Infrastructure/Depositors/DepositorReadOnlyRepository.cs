using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Depositors;

internal class DepositorReadOnlyRepository(ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IDepositorReadOnlyRepository
{
    public async Task<Depositor?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<Depositor>(
                x => x.Code == code.ToUpperInvariant(), cancellationToken);
    }
}