using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Nexticz.Lib.Shared.BackgroundServices;
using Nexticz.Lib.Shared.Generators;
using Nexticz.Module.Sign.DocumentLoader.Application.FileProcessors.Orchestrator;

namespace Nexticz.Module.Sign.DocumentLoader.Infrastructure;

internal class FileProcessorWorker(
    ILogger<FileProcessorWorker> logger,
    IServiceScopeFactory serviceScopeFactory,
    IFeatureManager featureManager) : BaseBackgroundService(featureManager, logger)
{
    private const string WorkerSignFtpLoaderWorkerEnabled = nameof(WorkerSignFtpLoaderWorkerEnabled);
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(7));
    private readonly IFeatureManager _featureManager = featureManager;

    protected override async Task ExecuteWorkerAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            if (!await _featureManager.IsEnabledAsync(nameof(WorkerSignFtpLoaderWorkerEnabled)))
                continue;
            
            var roundKey = Base62IdGenerator.GenerateId(10);
            try
            {
                var scope = serviceScopeFactory.CreateScope();
                var services = scope.ServiceProvider;
                var fileProcessorOrchestrator = services.GetRequiredService<IFileProcessorOrchestrator>();
                await fileProcessorOrchestrator.ProcessFilesAsync(roundKey, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SIGN - RoundKey: {RoundKey} - Error during file processing. Exception: {Exception}", 
                    roundKey, ex.Message);
            }
        }
    }
}