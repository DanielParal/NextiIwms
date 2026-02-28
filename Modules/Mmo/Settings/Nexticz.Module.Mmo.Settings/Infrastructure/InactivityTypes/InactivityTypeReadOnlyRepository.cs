using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.InactivityTypes;

internal class InactivityTypeReadOnlyRepository(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IInactivityTypeReadOnlyRepository
{
    public async Task<InactivityType?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFirstByConditionAsync<InactivityType>(
            x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase), cancellationToken);
    }
}