using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Nexticz.Lib.Shared.BackgroundServices;
using Nexticz.Lib.Shared.Generators;

namespace Nexticz.Lib.Shared.MassTransit.MessageData;

internal class MessageDataCleanupWorker(
    IFeatureManager featureManager, 
    ILogger<MessageDataCleanupWorker> logger,
    MessageDataSettings settings) : BaseBackgroundService(featureManager, logger)
{
    private const string WorkerMasstransitMessageDataCleanupEnabled = nameof(WorkerMasstransitMessageDataCleanupEnabled);
    private readonly PeriodicTimer _timer = new(TimeSpan.FromHours(1));
    private readonly IFeatureManager _featureManager = featureManager;
    
    protected override async Task ExecuteWorkerAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            if (!await _featureManager.IsEnabledAsync(nameof(WorkerMasstransitMessageDataCleanupEnabled)))
                continue;
            
            var roundKey = Base62IdGenerator.GenerateId(10);
            try
            {
                logger.LogInformation("[Start] [MessageDataCleanupWorker] - RoundKey: {RoundKey}", roundKey);
                
                var cutoff = DateTime.UtcNow - settings.TimeToLive;

                var files = Directory.EnumerateFiles(settings.BaseFolder, "*", SearchOption.AllDirectories).ToArray();
                var deletedFilesCount = 0;
                foreach (var file in files)
                {
                    if (File.GetCreationTimeUtc(file) < cutoff)
                    {
                        File.Delete(file);
                        deletedFilesCount++;
                    }
                }
                
                logger.LogInformation("[End] [MessageDataCleanupWorker] - RoundKey: {RoundKey}, deleted number of files: {Count}", roundKey, deletedFilesCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[Error] [MessageDataCleanupWorker] - RoundKey: {RoundKey}", 
                    roundKey);
            }
        }
    }
}