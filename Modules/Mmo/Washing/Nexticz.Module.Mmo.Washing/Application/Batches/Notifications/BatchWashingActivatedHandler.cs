using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines.Notifications;
using Nexticz.Module.Mmo.Washing.Application.Batches.Orchestrators.ActivateBatch;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Notifications;

internal class BatchWashingActivatedHandler(
    ILogger<BatchWashingActivatedHandler> logger,
    IActivateBatchOrchestrator activateBatchOrchestrator) : INotificationHandler<BatchWashingActivatedNotification>
{
    public async Task Handle(BatchWashingActivatedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Washing - Batch washing activated notification received. " +
                              "Finish batch Id: {FinishBatchId}, Finish sister batch id: {FinishSisterBatchId}, " +
                              "requested washing machine speed: {RequestedWashingMachineSpeed}, " +
                              "start batch id: {StartBatchId} on line: {LineCode}, " +
                              "start sister batch id: {SisterBatchId} on line: {SisterLineId}, at DateTimeOffset {DateActivated}.",
            notification.FinishedBatchId, notification.FinishedSisterBatchId, notification.RequestedWashingMachineSpeed,
            notification.ActivatedBatch.Batch.Id, notification.ActivatedBatch.LineCode, 
            notification.ActivatedSisterBatch?.Batch.Id, notification.ActivatedSisterBatch?.LineCode, 
            notification.DateActivated);
        
        var result = await activateBatchOrchestrator.OrchestrateAsync(
            notification.FinishedBatchId, notification.FinishedSisterBatchId, notification.RequestedWashingMachineSpeed,
            (SpeedLevel)notification.RequestedWashingMachineSpeedLevel, notification.ActivatedBatch, notification.ActivatedSisterBatch, 
            notification.DateActivated, cancellationToken);

        if (result.IsError)
        {
            logger.LogWarning("Washing - there was a problem activating batch id: {StartBatchId} on line: {LineCode} with, " +
                              "start sister batch id: {SisterBatchId} on line: {SisterLineId}, at DateTimeOffset {DateActivated}. " +
                              "Error Code: {ErrorCode}, Error Message: {ErrorMessage}.",
                notification.ActivatedBatch.Batch.Id, notification.ActivatedBatch.LineCode, 
                notification.ActivatedSisterBatch?.Batch.Id, notification.ActivatedSisterBatch?.LineCode,
                notification.DateActivated, result.FirstError.Code, result.FirstError.Description);

            return;
        }
        
        logger.LogInformation("Washing - Batch washing activated successfully. " +
                              "Finish batch Id: {FinishBatchId}, Finish sister batch id: {FinishSisterBatchId}, " +
                              "start batch id: {StartBatchId} on line: {LineCode}, " +
                              "start sister batch id: {SisterBatchId} on line: {SisterLineId}, at DateTimeOffset {DateActivated}.",
            notification.FinishedBatchId, notification.FinishedSisterBatchId, 
            notification.ActivatedBatch.Batch.Id, notification.ActivatedBatch.LineCode, 
            notification.ActivatedSisterBatch?.Batch.Id, notification.ActivatedSisterBatch?.LineCode, 
            notification.DateActivated);
    }
}