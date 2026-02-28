using Marten;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Packagings;

internal class PackagingReadOnlyRepository (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository)
    : IPackagingReadOnlyRepository
{
    public async Task<Packaging?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<Packaging>(
                x => 
                    x.Code.Equals(code, StringComparison.OrdinalIgnoreCase), 
                cancellationToken);
    }

    public async Task<IReadOnlyList<Packaging>> GetPackagingsByCodesAsync(string[] codes, CancellationToken cancellationToken)
    {
        var codesUpper = codes.Select(code => code.ToUpperInvariant()).ToArray();
        return await readOnlyEventStoreRepository
            .GetAllByConditionAsync<Packaging>(
                x => 
                    x.Code.IsOneOf(codesUpper), 
                cancellationToken);
    }

    public async Task<IReadOnlyList<Packaging>> GetPackagingsByDepositorCodeAsync(string depositorCode, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetAllByConditionAsync<Packaging>(
                x => x.DepositorCode.Equals(depositorCode, StringComparison.OrdinalIgnoreCase), 
                cancellationToken);
    }

    public async Task<IReadOnlyList<Packaging>> GetPackagingsByPackagingTypeCodeAsync(string packagingTypeCode, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetAllByConditionAsync<Packaging>(
                x => x.PackagingTypeCode.Equals(packagingTypeCode, StringComparison.OrdinalIgnoreCase), 
                cancellationToken);
    }

    public async Task<IReadOnlyList<Packaging>> GetPackagingsByPackagingCirculationCodeAsync(string packagingCirculationCode, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetAllByConditionAsync<Packaging>(
                x => x.PackagingCirculationCode.Equals(packagingCirculationCode, StringComparison.OrdinalIgnoreCase), 
                cancellationToken);
    }
}