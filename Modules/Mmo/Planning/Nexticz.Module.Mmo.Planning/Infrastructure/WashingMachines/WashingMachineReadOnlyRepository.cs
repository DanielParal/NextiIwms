using Marten;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Infrastructure.WashingMachines;

internal class WashingMachineReadOnlyRepository(IPlanningReadOnlyEventStoreRepository readOnlyRepository)
    : IWashingMachineReadOnlyRepository
{
    public async Task<WashingMachine?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await readOnlyRepository.GetFirstByConditionAsync<WashingMachine>(
            x => x.Code.Equals(code, StringComparison.InvariantCultureIgnoreCase), cancellationToken);
    }

    public async Task<WashingMachine?> GetByLineQueueCodeAsync(string lineQueueCode, CancellationToken cancellationToken)
    {
        return await readOnlyRepository.GetFirstByConditionAsync<WashingMachine>(
            x => x.LineQueues.Any(
                lq => lq.WashingMachineLineCode.Equals(lineQueueCode, StringComparison.InvariantCultureIgnoreCase)), 
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<WashingMachine>> GetAllByCodesAsync(string[] codes, CancellationToken cancellationToken)
    {
        var upperCaseCodes = codes.Select(x => x.ToUpperInvariant()).ToArray();
        return await readOnlyRepository.GetAllByConditionAsync<WashingMachine>(
            x => x.Code.IsOneOf(upperCaseCodes), cancellationToken);
    }
}