using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Nexticz.Lib.Shared.BackgroundServices;
using Nexticz.Lib.Shared.Generators;
using Nexticz.Module.EmailSender.Application.Orchestrators;

namespace Nexticz.Module.EmailSender.Infrastructure;

internal class ScheduledEmailsSenderWorker(
    ILogger<ScheduledEmailsSenderWorker> logger,
    IServiceScopeFactory serviceScopeFactory,
    IFeatureManager featureManager) : BaseBackgroundService(featureManager, logger)
{
    private const string WorkerEmailSenderScheduledEmailsSenderWorkerEnabled = nameof(WorkerEmailSenderScheduledEmailsSenderWorkerEnabled);
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(60));
    private readonly IFeatureManager _featureManager = featureManager;

    protected override async Task ExecuteWorkerAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            if (!await _featureManager.IsEnabledAsync(nameof(WorkerEmailSenderScheduledEmailsSenderWorkerEnabled)))
                continue;
            
            var roundKey = Base62IdGenerator.GenerateId(10);
            try
            {
                var scope = serviceScopeFactory.CreateScope();
                var services = scope.ServiceProvider;
                var sendEmailOrchestrator = services.GetRequiredService<ISendEmailOrchestrator>();
                await sendEmailOrchestrator.SendScheduledEmailsAsync(roundKey, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EmailSender - RoundKey: {RoundKey} - Error during sending scheduled emails. Exception: {Exception}", 
                    roundKey, ex.Message);
            }
        }
    }
}