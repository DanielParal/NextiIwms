using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Kits;

internal class KitReadOnlyRepository (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository)
    : IKitReadOnlyRepository
{
    public async Task<Kit?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<Kit>(
                x => 
                    x.Code == code.ToUpperInvariant(), 
                cancellationToken);
    }

    public async Task<IReadOnlyList<Kit>> GetKitsByKitTypeCodeAsync(string kitTypeCode, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetAllByConditionAsync<Kit>(
                x => x.KitTypeCode == kitTypeCode.ToUpperInvariant(), 
                cancellationToken);
    }

    public async Task<IReadOnlyList<Kit>> GetKitsByKitSapDefinitionCodeAsync(string kitSapDefinitionCode, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetAllByConditionAsync<Kit>(
                x => x.KitSapDefinitionCode == kitSapDefinitionCode.ToUpperInvariant(), 
                cancellationToken);
    }

    public async Task<IReadOnlyList<Kit>> GetKitsByManufactureCodeAsync(string manufactureCode, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetAllByConditionAsync<Kit>(
                x => x.ManufactureCode == manufactureCode.ToUpperInvariant(), 
                cancellationToken);
    }

    public async Task<IReadOnlyList<Kit>> GetKitsByDepositorCodeAsync(string depositorCode, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetAllByConditionAsync<Kit>(
                x => x.DepositorCode == depositorCode.ToUpperInvariant(), 
                cancellationToken);
    }

    public async Task<IReadOnlyList<Kit>> GetKitsByPackagingCode(string packagingCode, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetAllByConditionAsync<Kit>(
                k => k.PackagingCodeQuantities.Any(packagingCodeQuantity => packagingCodeQuantity.PackagingCode == packagingCode.ToUpperInvariant()), 
                cancellationToken);
    }

    public async Task<IReadOnlyList<Kit>> GetKitsBySpecialInformationId(Guid id, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetAllByConditionAsync<Kit>(
                k => k.SpecialInformationSchedules.Any(specInfo => specInfo.Id == id), 
                cancellationToken);
    }
}