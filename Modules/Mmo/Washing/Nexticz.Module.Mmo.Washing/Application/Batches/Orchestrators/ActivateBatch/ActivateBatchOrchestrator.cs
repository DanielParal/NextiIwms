using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Washing.Application.Batches.Commands.FinishBatchWashing;
using Nexticz.Module.Mmo.Washing.Application.Batches.Commands.StartBatchWashing;
using Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchByLineCode;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Orchestrators.ActivateBatch;

internal class ActivateBatchOrchestrator(
    ISender sender,
    IWashingUnitOfWork unitOfWork) : IActivateBatchOrchestrator
{
    public async Task<ErrorOr<Success>> OrchestrateAsync(Guid? finishedBatchId, Guid? finishedSisterBatchId, 
        int requestedWashingMachineSpeed,
        SpeedLevel requestedWashingMachineSpeedLevel,
        BatchActivatedResponse activatedBatch, BatchActivatedResponse? activatedSisterBatch, 
        DateTimeOffset dateActivated, CancellationToken cancellationToken)
    {
        unitOfWork.BeginTransaction();
        
        var currentBatchOnLine = await sender.Send(new GetBatchByLineCodeQuery(activatedBatch.LineCode), cancellationToken);
        var currentBatchOnSisterLine = activatedSisterBatch?.LineCode is null ? null : await sender.Send(new GetBatchByLineCodeQuery(activatedSisterBatch!.LineCode), cancellationToken);

        var resultFinishBatch = await TryFinishBatchAsync(finishedBatchId, dateActivated, cancellationToken);
        if (resultFinishBatch.IsError)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            return resultFinishBatch.Errors;
        }
        
        var resultFinishSisterBatch = await TryFinishBatchAsync(finishedSisterBatchId, dateActivated, cancellationToken);
        if (resultFinishSisterBatch.IsError)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            return resultFinishSisterBatch.Errors;
        }

        ErrorOr<Success> resultStart;
        if (activatedSisterBatch is null)
        {
            resultStart = await StartBatchAsync(currentBatchOnLine, requestedWashingMachineSpeed, requestedWashingMachineSpeedLevel, activatedBatch, cancellationToken);
        }
        else
        {
            resultStart = await StartSisterBatchesAsync(
                currentBatchOnLine, currentBatchOnSisterLine, requestedWashingMachineSpeed, requestedWashingMachineSpeedLevel,
                activatedBatch, activatedSisterBatch, cancellationToken);
        }

        if (resultStart.IsError)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            return resultStart.Errors;
        }
        
        await unitOfWork.CommitTransactionAsync(cancellationToken);
        return Result.Success;
    }

    private async Task<ErrorOr<Success>> StartSisterBatchesAsync(
        Batch? currentBatchOnLine,
        Batch? currentBatchOnSisterLine,
        int requestedWashingMachineSpeed,
        SpeedLevel requestedWashingMachineSpeedLevel,
        BatchActivatedResponse activatedBatch,
        BatchActivatedResponse activatedSisterBatch,
        CancellationToken cancellationToken)
    {
        var wasMachineAdjusted = WasMachineAdjusted(
            currentBatchOnLine, currentBatchOnSisterLine, activatedBatch, activatedSisterBatch);

        var shouldFirstKitStartAfterPreviousBatchLastKitEndDate = !wasMachineAdjusted;
        var previousBatchLastKitEndDate = GetPreviousBatchLastKitEndDate(currentBatchOnLine);
        var resultStartBatch = await sender.Send(
            new StartBatchWashingCommand(
                activatedBatch.Batch, 
                activatedBatch.WashingMachineCode,
                activatedBatch.LineCode,
                requestedWashingMachineSpeed,
                requestedWashingMachineSpeedLevel,
                activatedBatch.DateActivated,
                previousBatchLastKitEndDate,
                shouldFirstKitStartAfterPreviousBatchLastKitEndDate),
            cancellationToken);
        
        if (resultStartBatch.IsError)
            return resultStartBatch.Errors;
        
        var previousSisterBatchLastKitEndDate = GetPreviousBatchLastKitEndDate(currentBatchOnSisterLine);
        var resultSisterStartBatch = await sender.Send(
            new StartBatchWashingCommand(
                activatedSisterBatch.Batch, 
                activatedSisterBatch.WashingMachineCode,
                activatedSisterBatch.LineCode,
                requestedWashingMachineSpeed,
                requestedWashingMachineSpeedLevel,
                activatedSisterBatch.DateActivated,
                previousSisterBatchLastKitEndDate,
                shouldFirstKitStartAfterPreviousBatchLastKitEndDate),
            cancellationToken);
        
        if (resultSisterStartBatch.IsError)
            return resultSisterStartBatch.Errors;
        
        return Result.Success;
    }

    private static bool WasMachineAdjusted(
        Batch? currentBatchOnLine,
        Batch? currentBatchOnSisterLine,
        BatchActivatedResponse activatedBatch,
        BatchActivatedResponse activatedSisterBatch)
    {
        if (currentBatchOnLine is null || currentBatchOnSisterLine is null)
            return true;
        
        return currentBatchOnLine.PackagingHeight != activatedBatch.Batch.PackagingHeight ||
               currentBatchOnSisterLine.PackagingHeight != activatedSisterBatch.Batch.PackagingHeight;
    }

    private async Task<ErrorOr<Success>> StartBatchAsync(
        Batch? currentBatchOnLine, 
        int requestedWashingMachineSpeed,
        SpeedLevel requestedWashingMachineSpeedLevel,
        BatchActivatedResponse activatedBatch, 
        CancellationToken cancellationToken)
    {
        var shouldFirstKitStartAfterPreviousBatchLastKitEndDate = currentBatchOnLine?.PackagingHeight == activatedBatch.Batch.PackagingHeight;
        var previousBatchLastKitEndDate = GetPreviousBatchLastKitEndDate(currentBatchOnLine);
        
        return await sender.Send(
            new StartBatchWashingCommand(
                activatedBatch.Batch, 
                activatedBatch.WashingMachineCode,
                activatedBatch.LineCode,
                requestedWashingMachineSpeed,
                requestedWashingMachineSpeedLevel,
                activatedBatch.DateActivated,
                previousBatchLastKitEndDate,
                shouldFirstKitStartAfterPreviousBatchLastKitEndDate),
            cancellationToken);
    }

    private static DateTimeOffset? GetPreviousBatchLastKitEndDate(Batch? currentBatchOnLine)
    {
        if (currentBatchOnLine is null)
            return null;
        
        var lastKit = currentBatchOnLine.KitWashCycles.OrderBy(x => x.EndDate).LastOrDefault();
        
        if (lastKit is null)
            return currentBatchOnLine.PreviousBatchLastKitEndDate;
        
        return lastKit.EndDate;
    }

    private async Task<ErrorOr<Success>> TryFinishBatchAsync(Guid? batchId, DateTimeOffset dateFinished, CancellationToken cancellationToken)
    {
        if (batchId == null)
            return Result.Success;
        
        return await sender.Send(
            new FinishBatchWashingCommand(batchId.Value, dateFinished),
            cancellationToken);
    }
}