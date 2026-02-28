using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;
using Nexticz.Lib.Shared.BackgroundServices;
using Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatches;
using Nexticz.Module.Mmo.Washing.Application.WashingMachineSpeeds.Commands.CreateWashingMachineSpeed;
using Nexticz.Module.Mmo.Washing.Application.WashingMachineSpeeds.Commands.UpdateWashingMachineSpeed;
using Nexticz.Module.Mmo.Washing.Application.WashingMachineSpeeds.Queries.GetWashingMachineSpeeds;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.WashingMachineSpeeds;

internal class WashingMachineSpeedSyncWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<WashingMachineSpeedSyncWorker> logger,
    IFeatureManager featureManager) : BaseBackgroundService(featureManager, logger)
{
    private const string WorkerMmoWashingMachineSpeedSyncWorker = nameof(WorkerMmoWashingMachineSpeedSyncWorker);
    private readonly IFeatureManager _featureManager = featureManager;
    
    protected override async Task ExecuteWorkerAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!await _featureManager.IsEnabledAsync(nameof(WorkerMmoWashingMachineSpeedSyncWorker)))
                continue;
            
            using var scope = scopeFactory.CreateScope();

            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            var washingMachinesFromSettings = await sender.Send(new GetWashingMachineResponsesQuery(), stoppingToken);
            var batches = await sender.Send(new GetBatchesQuery(), stoppingToken);
            var washingMachineSpeeds = await sender.Send(new GetWashingMachineSpeedsQuery(), stoppingToken);

            await UpdateWashingMachineSpeedsAsync(washingMachinesFromSettings, batches, washingMachineSpeeds, sender, stoppingToken);
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private static async Task UpdateWashingMachineSpeedsAsync(WashingMachineResponse[] washingMachinesFromSettings, Batch[] batches, 
        WashingMachineSpeed[] currentSpeeds, ISender sender, CancellationToken stoppingToken)
    {
        foreach (var washingMachine in washingMachinesFromSettings)
        {
            var batchesWithCode = batches.Where(x => x.WashingMachineCode == washingMachine.Code).ToList();

            
            var batchWithMinSpeed = batchesWithCode.Count == 0 ? null : batchesWithCode.MinBy(x => x.PackagingSpeed);
            var minSpeed = batchWithMinSpeed?.PackagingSpeed ?? 0;
            var minSpeedLevel = batchWithMinSpeed?.PackagingSpeedLevel ?? SpeedLevel.NotSet;
            
            var currentSpeed = currentSpeeds.FirstOrDefault(x => x.Code == washingMachine.Code);

            if (currentSpeed == null)
            {
                await sender.Send(new CreateWashingMachineSpeedCommand(washingMachine.Code, minSpeed, minSpeedLevel), stoppingToken);
                continue;
            }
            
            if (currentSpeed.Speed != minSpeed)
            {
                await sender.Send(new UpdateWashingMachineSpeedCommand(currentSpeed.Id, currentSpeed.Code, minSpeed, minSpeedLevel), stoppingToken);
                continue;
            }
        }
    }
}