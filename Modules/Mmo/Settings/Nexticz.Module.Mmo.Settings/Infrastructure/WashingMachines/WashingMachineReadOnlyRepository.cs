using Marten;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.WashingMachines;

internal class WashingMachineReadOnlyRepository (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IWashingMachineReadOnlyRepository
{
    public async Task<WashingMachine?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<WashingMachine>(
                x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase), cancellationToken);
    }
    
    public async Task<WashingMachine?> GetByLineCodeAsync(string lineCode, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<WashingMachine>(
                x => x.WashingMachineLines.Any(l => l.Code.Equals(lineCode, StringComparison.InvariantCultureIgnoreCase)), cancellationToken);
    }

    public async Task<IReadOnlyList<WashingMachine>> GetWashingMachinesByCodesAsync(string[] codes, CancellationToken cancellationToken)
    {
        var codesUpper = codes.Select(code => code.ToUpperInvariant()).ToArray();
        return await readOnlyEventStoreRepository
            .GetAllByConditionAsync<WashingMachine>(
                x => 
                    x.Code.IsOneOf(codesUpper),
                cancellationToken);
    }
}