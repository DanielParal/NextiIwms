using Nexticz.Module.Mmo.Washing.Domain.WorkerEntity;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.LastEnteredWorkerOnLineAggregate;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.LastEnteredWorkerOnLines;

internal class LastEnteredWorkerOnLineRepository(
    IWashingReadOnlyEventStoreRepository readOnlyEventStoreRepository)
    : ILastEnteredWorkerOnLineRepository
{
    public async Task<LastEnteredWorkerOnLine?> GetLastEnteredWorkerOnLineAsync(string lineCode, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFirstByConditionAsync<LastEnteredWorkerOnLine>(
            x => x.LineCode.Equals(lineCode, StringComparison.InvariantCultureIgnoreCase), cancellationToken);
    }
}