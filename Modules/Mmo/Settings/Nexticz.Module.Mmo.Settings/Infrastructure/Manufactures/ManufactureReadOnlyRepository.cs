using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Manufactures;

internal class ManufactureReadOnlyRepository (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IManufactureReadOnlyRepository
{
    public async Task<Manufacture?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<Manufacture>(
                x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase), cancellationToken);
    }
}