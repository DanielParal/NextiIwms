using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.PackagingCirculations;

internal class PackagingCirculationReadOnlyRepository (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IPackagingCirculationReadOnlyRepository
{
    public async Task<PackagingCirculation?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<PackagingCirculation>(
                x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase), cancellationToken);
    }
}