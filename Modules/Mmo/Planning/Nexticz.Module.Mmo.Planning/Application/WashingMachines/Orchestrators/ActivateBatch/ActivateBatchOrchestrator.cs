using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines.Notifications;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings.Queries;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;
using Nexticz.Module.Mmo.Washing.Contracts.WashingMachineSpeeds.Queries;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.FinishBatch;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.StartBatchWashing;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using WashingMachineResponse = Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.WashingMachineResponse;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Orchestrators.ActivateBatch;

internal class ActivateBatchOrchestrator(
    ISender sender,
    IClock clock,
    IPlanningUnitOfWork unitOfWork,
    ILogger<ActivateBatchOrchestrator> logger,
    IPlanningNotificationCollector notificationCollector) : IActivateBatchOrchestrator
{
    public async Task<ErrorOr<ActivateBatchResponse>> OrchestrateAsync(
        Batch batch, string washingMachineCode, string upperLineQueueCode, 
        Batch? currentBatchInWashing, string currentBatchInWashingLineCode, 
        Batch? currentBatchInWashingInOtherLine, string? currentBatchInWashingInOtherLineLineCode,
        CancellationToken cancellationToken)
    {
        var dateActivated = clock.UtcNowOffset;
        Guid? finishedBatchId = null;
        Guid? finishedBatchInOtherLineId = null;
        unitOfWork.BeginTransaction();
        
        if (currentBatchInWashing is not null)
        {
            var resultFinishBatch = 
                await sender.Send(
                    new FinishBatchCommand(currentBatchInWashing.Id, currentBatchInWashingLineCode, dateActivated), 
                    cancellationToken);
            if (resultFinishBatch.IsError)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                notificationCollector.Clear();
                return resultFinishBatch.Errors;
            }

            finishedBatchId = resultFinishBatch.Value.FinishedBatchId;
            finishedBatchInOtherLineId = resultFinishBatch.Value.FinishedSisterBatchId;
        }
        
        if (currentBatchInWashingInOtherLine is not null && currentBatchInWashingInOtherLineLineCode is not null)
        {
            var resultFinishBatchInOtherLine = 
                await sender.Send(
                    new FinishBatchCommand(currentBatchInWashingInOtherLine.Id, currentBatchInWashingInOtherLineLineCode, dateActivated), 
                    cancellationToken);
            if (resultFinishBatchInOtherLine.IsError)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                notificationCollector.Clear();
                return resultFinishBatchInOtherLine.Errors;
            }
            
            finishedBatchInOtherLineId = resultFinishBatchInOtherLine.Value.FinishedBatchId;
        }
    
        var resultStartWashing = 
            await sender.Send(new StartBatchWashingCommand(batch.Id, upperLineQueueCode, dateActivated), cancellationToken);
        if (resultStartWashing.IsError)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            notificationCollector.Clear();
            return resultStartWashing.Errors;
        }
        
        var response = await GetActivateBatchResponseAsync(
            washingMachineCode,  
            batch.Id,
            resultStartWashing.Value.BatchStarted.Batch.PackagingCode, 
            resultStartWashing.Value.SisterBatchStarted?.Batch?.PackagingCode,
            cancellationToken);
        
        if (response.IsError)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            notificationCollector.Clear();
            return response.Errors;
        }
        
        notificationCollector.AddNotification(new BatchWashingActivatedNotification(
            finishedBatchId, finishedBatchInOtherLineId,
            response.Value.RequestedSpeed,
            response.Value.RequestedSpeedLevel,
            resultStartWashing.Value.BatchStarted, resultStartWashing.Value.SisterBatchStarted,
            dateActivated));
        
        await unitOfWork.CommitTransactionAsync(cancellationToken);
        await notificationCollector.PublishNotificationsAsync(cancellationToken);
        
        return response.Value;
    }

    private async Task<ErrorOr<ActivateBatchResponse>> GetActivateBatchResponseAsync(string washingMachineCode, Guid batchId, string packagingCode, string? sisterPackagingCode, CancellationToken cancellationToken)
    {
        var washingMachineFromSettings =
            await GetWashingMachineFromSettingsAsync(washingMachineCode, cancellationToken);
        if (washingMachineFromSettings.IsError)
            return washingMachineFromSettings.Errors;
        
        var speedResponse = await GetRequestedWashingMachineSpeedAsync(packagingCode, sisterPackagingCode, washingMachineFromSettings.Value, batchId, cancellationToken);
        if (speedResponse.IsError)
            return speedResponse.Errors;

        var currentWashingMachineSpeed = await sender.Send(new GetWashingMachineSpeedContractByCodeQuery(washingMachineCode), cancellationToken);
        
        return new ActivateBatchResponse(currentWashingMachineSpeed?.Speed, speedResponse.Value.Speed, speedResponse.Value.SpeedLevel);
    }
    
    private async Task<ErrorOr<WashingMachineResponse>> GetWashingMachineFromSettingsAsync(string washingMachineCode, CancellationToken cancellationToken)
    {
        var washingMachineFromSettings = await sender.Send(new GetWashingMachineResponseByCodeQuery(washingMachineCode), cancellationToken);

        if (washingMachineFromSettings.IsError)
        {
            logger.LogError("MMO - Planning - Cannot get washing machine from settings because washing machine with code: {WashingMachineCode} does not exist.", 
                washingMachineCode);
            return WashingMachineErrors.ValidationWashingMachineDoesNotExist;
        }
        
        return washingMachineFromSettings.Value;
    }
    
    private async Task<ErrorOr<(int Speed, SpeedLevelContract SpeedLevel)>> GetRequestedWashingMachineSpeedAsync(string packagingCode, string? sisterPackagingCode, WashingMachineResponse washingMachineResponse, Guid batchId,
        CancellationToken cancellationToken)
    {
        var packagingFromSettings = await sender.Send(new GetPackagingResponseByCodeQuery(packagingCode), cancellationToken);

        if (packagingFromSettings.IsError)
        {
            logger.LogError("MMO - Planning - Cannot get packaging speed for packaging with code: {PackagingCode}, batchId: {BatchId}. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}.", 
                packagingCode, batchId, packagingFromSettings.FirstError.Code, packagingFromSettings.FirstError.Description);
            return packagingFromSettings.Errors;
        }
        
        var requestedWashingMachineSpeedForLine1 = GetRequestedWashingMachineSpeed(packagingFromSettings.Value, washingMachineResponse);
        if (requestedWashingMachineSpeedForLine1.IsError)
            return requestedWashingMachineSpeedForLine1.Errors;
        
        if (sisterPackagingCode is null)
            return requestedWashingMachineSpeedForLine1.Value;
        
        var sisterPackagingFromSettings = await sender.Send(new GetPackagingResponseByCodeQuery(sisterPackagingCode), cancellationToken);
        if (sisterPackagingFromSettings.IsError)
        {
            logger.LogError("MMO - Planning - Cannot get sister packaging speed for packaging with code: {PackagingCode}, batchId: {BatchId}. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}.", 
                sisterPackagingCode, batchId, sisterPackagingFromSettings.FirstError.Code, sisterPackagingFromSettings.FirstError.Description);
            return sisterPackagingFromSettings.Errors;
        }
        
        var requestedWashingMachineSpeedForLine2 = GetRequestedWashingMachineSpeed(sisterPackagingFromSettings.Value, washingMachineResponse);
        if (requestedWashingMachineSpeedForLine2.IsError)
            return requestedWashingMachineSpeedForLine2.Errors;
        
        return requestedWashingMachineSpeedForLine1.Value.Speed >= requestedWashingMachineSpeedForLine2.Value.Speed ? requestedWashingMachineSpeedForLine1.Value : requestedWashingMachineSpeedForLine2.Value;
    }
    
    private ErrorOr<(int Speed, SpeedLevelContract SpeedLevel)> GetRequestedWashingMachineSpeed(PackagingResponse packagingResponse, WashingMachineResponse washingMachineResponse)
    {
        var packagingSpeedLevel = packagingResponse.WashingMachineSpeeds.FirstOrDefault(x => x.WashingMachineCode == washingMachineResponse.Code)?.Speed;
        if (packagingSpeedLevel is null)
        {
            logger.LogError(
                "MMO - Planning - Invalid speed level: speed for washing machine with code: {WashingMachineCode} does not exist. Packaging code: {PackagingCode}.",
                washingMachineResponse.Code, packagingResponse.Code);
            return WashingMachineErrors.ValidationInvalidSpeedLevel;
        } 
        
        var propertyName = packagingSpeedLevel.Value.ToString();
        
        var speedProperty = washingMachineResponse.GetType().GetProperty(propertyName);
        if (speedProperty == null) 
        {
            logger.LogError(
                "MMO - Planning - Invalid speed level: {SpeedLevel} requested for washing machine with code: {Code}.",
                packagingSpeedLevel.Value, washingMachineResponse.Code);
            return WashingMachineErrors.ValidationInvalidSpeedLevel;
        }
        
        if (speedProperty.GetValue(washingMachineResponse) is int speedValue)
        {
            return (speedValue, (SpeedLevelContract)packagingSpeedLevel.Value);
        }
    
        logger.LogError(
            "MMO - Planning - Washing machine with code: {Code} does not contain a valid speed value for level {SpeedLevel}.",
            washingMachineResponse.Code, packagingSpeedLevel.Value);
        return WashingMachineErrors.ValidationInvalidSpeedLevel;
        
    }
}