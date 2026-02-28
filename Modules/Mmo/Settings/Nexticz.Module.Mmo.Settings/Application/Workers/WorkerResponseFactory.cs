using Nexticz.Module.Mmo.Settings.Contracts.Workers;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Workers;

internal class WorkerResponseFactory
{
    public static WorkerResponse Create(Worker worker)
    {
        return new WorkerResponse(
            worker.Id,
            worker.Name,
            worker.Pin,
            worker.IsActive);
    }
}