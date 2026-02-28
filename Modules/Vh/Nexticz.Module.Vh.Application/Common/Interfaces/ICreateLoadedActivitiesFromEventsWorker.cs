namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface ICreateLoadedActivitiesFromEventsWorker
{
    Task ExecuteWorkAsync(CancellationToken stoppingToken);
}