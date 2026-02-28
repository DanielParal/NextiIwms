using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Nexticz.Lib.Shared.BackgroundServices;
using Nexticz.Lib.Shared.Generators;
using Nexticz.Module.Cuzk.Application.ImportsExports.Imports.Orchestrators;

namespace Nexticz.Module.Cuzk.Infrastructure.Imports;

internal class ImportProcessorWorker(
    ILogger<ImportProcessorWorker> logger,
    IServiceScopeFactory serviceScopeFactory,
    IFeatureManager featureManager) : BaseBackgroundService(featureManager, logger)
{
    
    private const string WorkerCuzkImportProcessorWorkerEnabled = nameof(WorkerCuzkImportProcessorWorkerEnabled);
    private readonly IFeatureManager _featureManager = featureManager;
    
    protected override async Task ExecuteWorkerAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!await _featureManager.IsEnabledAsync(nameof(WorkerCuzkImportProcessorWorkerEnabled)))
                continue;
            
            var roundKey = Base62IdGenerator.GenerateId(10);
            try
            {
                var scope = serviceScopeFactory.CreateScope();
                var services = scope.ServiceProvider;
                var importProcessorOrchestrator = services.GetRequiredService<IImportProcessorOrchestrator>();
                await importProcessorOrchestrator.ProcessImportsAsync(roundKey, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "[Cuzk] [ERROR] [ProcessImportsWorker] - RoundKey: {RoundKey} - Error during file processing. Exception: {Exception}",
                    roundKey, ex.Message);
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }
}