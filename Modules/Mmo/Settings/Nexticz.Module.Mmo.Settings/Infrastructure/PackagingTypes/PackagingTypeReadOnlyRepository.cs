using Marten;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.PackagingTypes;

internal class PackagingTypeReadOnlyRepository(ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository)
    : IPackagingTypeReadOnlyRepository
{
    public async Task<PackagingType?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<PackagingType>(
                x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase), cancellationToken);
    }

    public async Task<IReadOnlyList<PackagingType>> GetPackagingTypesByCodesAsync(string[] codes, CancellationToken cancellationToken)
    {
        var codesUpper = codes.Select(code => code.ToUpperInvariant()).ToArray();
        return await readOnlyEventStoreRepository
            .GetAllByConditionAsync<PackagingType>(
                x => 
                    x.Code.IsOneOf(codesUpper), 
                cancellationToken);
    }
}