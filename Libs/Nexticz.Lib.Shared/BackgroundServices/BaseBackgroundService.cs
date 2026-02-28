using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace Nexticz.Lib.Shared.BackgroundServices;

public abstract class BaseBackgroundService(
    IFeatureManager featureManager,
    ILogger<BaseBackgroundService> logger) : BackgroundService
{
    private const string WorkersEnabled = nameof(WorkersEnabled);
    private const int WorkersDelayedStartInSeconds = 60;
    
    protected abstract Task ExecuteWorkerAsync(CancellationToken stoppingToken);
    
    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await featureManager.IsEnabledAsync(nameof(WorkersEnabled)))
        {
            logger.LogWarning("Feature {WorkersEnabled} is disabled. No workers are running.", WorkersEnabled);
            return;
        }
        
        await Task.Delay(TimeSpan.FromSeconds(WorkersDelayedStartInSeconds), stoppingToken);
        await ExecuteWorkerAsync(stoppingToken);
    }
}