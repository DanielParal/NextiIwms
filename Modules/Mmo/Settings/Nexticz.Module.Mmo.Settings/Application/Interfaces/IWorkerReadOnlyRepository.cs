using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Interfaces;

internal interface IWorkerReadOnlyRepository
{
    Task<Worker?> GetByPinAsync(int pin, CancellationToken cancellationToken);
}