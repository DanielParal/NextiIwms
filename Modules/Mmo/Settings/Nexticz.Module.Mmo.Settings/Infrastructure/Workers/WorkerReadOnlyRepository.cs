using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Workers;

internal class WorkerReadOnlyRepository(ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository)
    : IWorkerReadOnlyRepository
{
    public async Task<Worker?> GetByPinAsync(int pin, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<Worker>(
                x => x.Pin == pin, cancellationToken);
    }
}