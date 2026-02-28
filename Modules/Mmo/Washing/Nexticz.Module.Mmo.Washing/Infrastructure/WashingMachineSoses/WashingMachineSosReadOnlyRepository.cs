using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.WashingMachineSoses;

internal class WashingMachineSosReadOnlyRepository(
    IWashingReadOnlyEventStoreRepository washingReadOnlyEventStoreRepository) : IWashingMachineSosReadOnlyRepository
{
    public async Task<WashingMachineSos?> GetWashingMachineSosByCodeAsync(string washingMachineCode, CancellationToken cancellationToken)
    {
        return await washingReadOnlyEventStoreRepository
            .GetFirstByConditionAsync<WashingMachineSos>(
                x => x.Code.Equals(washingMachineCode, StringComparison.InvariantCultureIgnoreCase), 
                cancellationToken);
    }
}