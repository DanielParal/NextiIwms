using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Nexticz.Lib.Shared.BackgroundServices;

public class OnDemandBackgroundServiceHost<T>(
    ILogger<OnDemandBackgroundServiceHost<T>> logger,
    IServiceProvider serviceProvider) where T : IOnDemandBackgroundService
{
    private CancellationTokenSource? _cts;
    private readonly Lock _lock = new();
    
    private static string CurrentWorkerName => typeof(T).Name;

    private bool IsRunning { get; set; }

    public void Start()
    {
        lock (_lock)
        {
            if (IsRunning)
            {
                logger.LogInformation("{BaseClassName} - Worker {WorkerName} is already running.", 
                    nameof(OnDemandBackgroundServiceHost<T>), CurrentWorkerName);
                return;
            }

            _cts = new CancellationTokenSource();
            Task.Run(async () => await RunAsync(_cts.Token));
            IsRunning = true;
            logger.LogInformation("{BaseClassName} - Worker {WorkerName} started.", 
                nameof(OnDemandBackgroundServiceHost<T>), CurrentWorkerName);
        }
    }

    public void Stop()
    {
        lock (_lock)
        {
            if (!IsRunning || _cts == null)
            {
                logger.LogInformation("{BaseClassName} - Worker {WorkerName} is not running.", 
                    nameof(OnDemandBackgroundServiceHost<T>), CurrentWorkerName);
                return;
            }

            _cts.Cancel();
            logger.LogInformation("{BaseClassName} - Worker {WorkerName} stop requested.", 
                nameof(OnDemandBackgroundServiceHost<T>), CurrentWorkerName);
        }
    }

    private async Task RunAsync(CancellationToken token)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<T>();
            await service.DoWorkAsync(token);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("{BaseClassName} - Worker {WorkerName} cancelled.",
                nameof(OnDemandBackgroundServiceHost<T>), CurrentWorkerName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{BaseClassName} - Worker {WorkerName} encountered an error.", 
                nameof(OnDemandBackgroundServiceHost<T>), CurrentWorkerName);
        }
        finally
        {
            lock (_lock)
            {
                IsRunning = false;
            }
            logger.LogInformation("{BaseClassName} - Worker {WorkerName} has stopped.", 
                nameof(OnDemandBackgroundServiceHost<T>), CurrentWorkerName);
        }
    }
}