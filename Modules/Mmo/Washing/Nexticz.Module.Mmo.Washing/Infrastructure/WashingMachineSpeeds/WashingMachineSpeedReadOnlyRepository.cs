using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.WashingMachineSpeeds;

internal class WashingMachineSpeedReadOnlyRepository(
    IWashingReadOnlyEventStoreRepository readOnlyRepository)
    : IWashingMachineSpeedReadOnlyRepository
{
    public async Task<WashingMachineSpeed?> GetWashingMachineSpeedByCodeAsync(string washingMachineCode, CancellationToken cancellationToken)
    {
        return await readOnlyRepository.GetFirstByConditionAsync<WashingMachineSpeed>(
            x => x.Code.Equals(washingMachineCode, StringComparison.InvariantCultureIgnoreCase), cancellationToken);
    }
}