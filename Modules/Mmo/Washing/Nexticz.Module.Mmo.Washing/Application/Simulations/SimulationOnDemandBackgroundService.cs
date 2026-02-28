using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;
using Nexticz.Module.Mmo.Washing.Contracts.WashingStates;
using Nexticz.Lib.Shared.BackgroundServices;
using Nexticz.Module.Mmo.Washing.Application.Batches.Commands.FinishKit;
using Nexticz.Module.Mmo.Washing.Application.LastEnteredWorkerOnLines.Commands.EnterLine;
using Nexticz.Module.Mmo.Washing.Application.WashingStates.Queries.GetWashingStateResponse;

namespace Nexticz.Module.Mmo.Washing.Application.Simulations;

internal class SimulationOnDemandBackgroundService(
    ILogger<SimulationOnDemandBackgroundService> logger,
    IFeatureManager featureManager,
    ISender sender) 
    : IOnDemandBackgroundService
{
    private const string MmoWashingSimulation = nameof(MmoWashingSimulation);
    // For simplicity workers with those two pins have to be created in the database
    public const int HardCodedWorker1Pin = 1111;
    public const int HardCodedWorker2Pin = 2222;

    public async Task DoWorkAsync(CancellationToken token)
    {
        if (!await featureManager.IsEnabledAsync(nameof(MmoWashingSimulation)))
        {
            logger.LogInformation("Washing - {WorkerName} Simulating feature is not enabled...", 
                nameof(SimulationOnDemandBackgroundService));
            return;
        }
        
        var enterLinesResult = await EnterWorkersToLinesAsync(token);
        if (enterLinesResult.IsError)
        {
            logger.LogWarning("Washing - {WorkerName} cannot enter workers to lines. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}.", 
                nameof(SimulationOnDemandBackgroundService), enterLinesResult.FirstError.Code, enterLinesResult.FirstError.Description);
            return;
        }
        
        while (!token.IsCancellationRequested)
        {
            var washingStateResponse = await sender.Send(new GetWashingStateResponseQuery(), token);
            await FinishKitsAsync(washingStateResponse.WashingMachines, token);
            
            logger.LogInformation("Washing - {WorkerName} Simulating next round finished...", nameof(SimulationOnDemandBackgroundService));
            
            var delay = TimeSpan.FromSeconds(new Random().Next(180, 240));
            await Task.Delay(delay, token);
        }
    }

    private async Task FinishKitsAsync(WashingStateMachineContract[] washingMachines, CancellationToken token)
    {
        foreach (var machine in washingMachines)
        {
            var lines = machine.Lines;

            if (lines.Length == 1)
            {
                var batch = lines[0].Batch;
                if (batch is not null)
                {
                    await sender.Send(new FinishKitCommand(batch.Id), token);
                }

                continue;
            }

            var firstBatch = lines[0].Batch;
            var secondBatch = lines[1].Batch;

            if (firstBatch is not null)
            {
                await sender.Send(new FinishKitCommand(firstBatch.Id), token);

                if (firstBatch.SisterBatchId is not null)
                    continue;
            }

            if (secondBatch is not null)
            {
                await sender.Send(new FinishKitCommand(secondBatch.Id), token);
            }
        }
    }

    private async Task<ErrorOr<Success>> EnterWorkersToLinesAsync(CancellationToken token)
    {
        var washingMachinesFromSettings = await sender.Send(new GetWashingMachineResponsesQuery(), token);

        foreach (var washingMachine in washingMachinesFromSettings)
        {
            var resultLine1 = await sender.Send(new EnterLineCommand(washingMachine.WashingMachineLines[0].Code, HardCodedWorker1Pin), token);
            if (resultLine1.IsError)
                return resultLine1.Errors;
            
            if (washingMachine.NumberOfLines > 1)
            {
                var resultLine2 = await sender.Send(new EnterLineCommand(washingMachine.WashingMachineLines[1].Code, HardCodedWorker2Pin), token);
                if (resultLine2.IsError)
                    return resultLine2.Errors;
            }
        }
        
        return Result.Success;
    }
}