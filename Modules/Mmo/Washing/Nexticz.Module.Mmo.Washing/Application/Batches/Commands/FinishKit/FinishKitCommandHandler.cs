using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Washing.Application.WashingMachineSpeeds.Queries.GetWashingMachineSpeeds;
using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Module.Mmo.Washing.Contracts.Batches.Notifications;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Washing.Application.DoubleClickProtectors;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Application.LastEnteredWorkerOnLines.Queries.GetLastWorkerByLineCode;
using Nexticz.Module.Mmo.Washing.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Washing.Application.WashingMachineSpeeds.Queries.GetWashingMachineSpeedByCode;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;
using Nexticz.Module.Mmo.Washing.Domain.KitWashCycleEntity;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;
using Nexticz.Module.Mmo.Washing.Domain.WorkerEntity;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Commands.FinishKit;

internal class FinishKitCommandHandler(
    ILogger<FinishKitCommandHandler> logger,
    ISender sender,
    IWashingReadOnlyEventStoreRepository readOnlyEventStoreRepository,
    IWashingUnitOfWork unitOfWork,
    IClock clock,
    IWashingNotificationCollector notificationCollector,
    IGlobalKitsCounter globalKitsCounter,
    IDoubleClickProtector doubleClickProtector) 
    : IRequestHandler<FinishKitCommand, ErrorOr<KitWashCycle>>
{
    public async Task<ErrorOr<KitWashCycle>> Handle(FinishKitCommand request, CancellationToken cancellationToken)
    {
        var batch = await GetBatchAsync(request.BatchId, cancellationToken);
        
        var validationResult = await ValidateBatchAsync(batch, cancellationToken);
        if (validationResult.IsError)
            return validationResult.Errors;
        
        var washingMachineSpeed = 
            await sender.Send(new GetWashingMachineSpeedByCodeQuery(batch.Value.WashingMachineCode), cancellationToken);
        var washingMachineSpeedInt = washingMachineSpeed?.Speed ?? 0;
        var washingMachineSpeedLevel = washingMachineSpeed?.SpeedLevel ?? SpeedLevel.NotSet;

        if (!await doubleClickProtector.CanFinishKitAsync(validationResult.Value.Batch, validationResult.Value.WorkerName))
            return BatchErrors.ValidationCannotFinishKitInShortTimePeriod;
            
        var now = clock.UtcNowOffset;
        if (validationResult.Value.Batch.SisterBatchId is not null)
        {
            var sisterValidationResult = 
                await ValidateSisterBatchAsync(validationResult.Value.Batch.SisterBatchId.Value, batch.Value, validationResult.Value.WorkerName, cancellationToken);
            if (sisterValidationResult.IsError)
                return sisterValidationResult.Errors; 
            
            var finishedKitWashCycles = 
                FinishSisterBatches(validationResult.Value, sisterValidationResult.Value, now, washingMachineSpeedInt, washingMachineSpeedLevel);
            return finishedKitWashCycles.kitWashCycle;
        }
        
        return FinishSingleBatch(validationResult.Value, now, washingMachineSpeedInt, washingMachineSpeedLevel);
    }
    
    private (KitWashCycle kitWashCycle, KitWashCycle sisterKitWashCycle) FinishSisterBatches(
        ValidationResult validationResult, 
        ValidationResult sisterValidationResult, 
        DateTimeOffset dateFinished,
        int washingMachineSpeed,
        SpeedLevel washingMachineSpeedLevel)
    {
        var startDate = validationResult.Batch.GetNextKitWashCycleStartDate(dateFinished);
        var sisterStartDate = sisterValidationResult.Batch.GetNextKitWashCycleStartDate(dateFinished);
        
        var minStartDate = startDate > sisterStartDate ? sisterStartDate : startDate;
        
        var orderId = validationResult.Batch.GetNextOrderId();
        var globalKitsCount = globalKitsCounter.GetAndIncrementCurrentValue();
        var kitId = Guid.NewGuid();
        var sisterKitId = Guid.NewGuid();
        var kitWashCycle = new KitWashCycle(sisterKitId, validationResult.Batch.Id, minStartDate, dateFinished, 
            validationResult.Batch.OptimalKitDuration, validationResult.WorkerName, orderId, globalKitsCount, washingMachineSpeed, washingMachineSpeedLevel, kitId);
        // Batch events
        var kitWashCycleFinishedEvent = 
            new KitWashCycleFinishedEvent(kitWashCycle.Id, kitWashCycle.SisterKitId, kitWashCycle.BatchId, kitWashCycle.StartDate, kitWashCycle.EndDate, 
                kitWashCycle.OptimalDuration, kitWashCycle.WorkerName, kitWashCycle.OrderId, kitWashCycle.GlobalKitsCount, 
                kitWashCycle.Efficiency, kitWashCycle.WashingMachineSpeed, kitWashCycle.WashingMachineSpeedLevel);
        unitOfWork.AppendEvent(validationResult.Batch.Id, kitWashCycleFinishedEvent);
        
        
        var sisterOrderId = sisterValidationResult.Batch.GetNextOrderId();
        var sisterKitWashCycle = new KitWashCycle(kitId, sisterValidationResult.Batch.Id, minStartDate, dateFinished, 
            sisterValidationResult.Batch.OptimalKitDuration, sisterValidationResult.WorkerName, sisterOrderId, globalKitsCount,
            washingMachineSpeed, washingMachineSpeedLevel, sisterKitId);
        // Sister batch events
        var sisterKitWashCycleFinishedEvent = 
            new KitWashCycleFinishedEvent(sisterKitWashCycle.Id, sisterKitWashCycle.SisterKitId, sisterKitWashCycle.BatchId, sisterKitWashCycle.StartDate, sisterKitWashCycle.EndDate, 
                sisterKitWashCycle.OptimalDuration, sisterKitWashCycle.WorkerName, sisterKitWashCycle.OrderId, sisterKitWashCycle.GlobalKitsCount, 
                sisterKitWashCycle.Efficiency, sisterKitWashCycle.WashingMachineSpeed, sisterKitWashCycle.WashingMachineSpeedLevel);
        unitOfWork.AppendEvent(sisterValidationResult.Batch.Id, sisterKitWashCycleFinishedEvent);

        var kitCounterIncrementedEvent = new KitCounterIncremented(kitWashCycle.Id,
            kitWashCycle.BatchId, sisterKitWashCycle.Id, sisterKitWashCycle.BatchId,
            globalKitsCount);
        unitOfWork.AppendEvent(validationResult.Batch.Id, kitCounterIncrementedEvent);
        
        // Batch notification
        notificationCollector.AddNotification(
            new KitWashingFinishedNotification(
                GetKitWashingFinishedContract(
                    kitWashCycle, validationResult.Batch, validationResult.WorkerName, 
                    minStartDate, dateFinished, globalKitsCount),
                GetKitWashingFinishedContract(
                    sisterKitWashCycle, sisterValidationResult.Batch, sisterValidationResult.WorkerName, 
                    minStartDate, dateFinished, globalKitsCount)
                ));
        
        logger.LogInformation("Washing - Sister kit wash cycle finished with id: {KitWashCycleId} in batch with id: {BatchId} " +
                              "and sister kit wash cycle id: {SisterKitWashCycleId} in sister batch with id: {SisterBatchId}",
            kitWashCycle.Id, kitWashCycle.BatchId, sisterKitWashCycle.Id, sisterKitWashCycle.BatchId);
        
        return (kitWashCycle, sisterKitWashCycle);
    }

    private KitWashCycle FinishSingleBatch(
        ValidationResult validationResult, DateTimeOffset dateFinished,
        int washingMachineSpeed,
        SpeedLevel washingMachineSpeedLevel)
    {
        var startDate = validationResult.Batch.GetNextKitWashCycleStartDate(dateFinished);
        var orderId = validationResult.Batch.GetNextOrderId();
        var completedKitsCount = globalKitsCounter.GetAndIncrementCurrentValue();
        var kitWashCycle = new KitWashCycle(null, validationResult.Batch.Id, startDate, 
            dateFinished, validationResult.Batch.OptimalKitDuration, validationResult.WorkerName, orderId, 
            completedKitsCount, washingMachineSpeed, washingMachineSpeedLevel);
        var kitWashCycleFinishedEvent = 
            new KitWashCycleFinishedEvent(kitWashCycle.Id, null, kitWashCycle.BatchId, kitWashCycle.StartDate, kitWashCycle.EndDate, 
                kitWashCycle.OptimalDuration, kitWashCycle.WorkerName, kitWashCycle.OrderId, 
                kitWashCycle.GlobalKitsCount, kitWashCycle.Efficiency, kitWashCycle.WashingMachineSpeed, kitWashCycle.WashingMachineSpeedLevel);
        unitOfWork.AppendEvent(validationResult.Batch.Id, kitWashCycleFinishedEvent);
        
        var kitCounterIncrementedEvent = new KitCounterIncremented(kitWashCycle.Id,
            kitWashCycle.BatchId, null, null, kitWashCycle.GlobalKitsCount);
        unitOfWork.AppendEvent(validationResult.Batch.Id, kitCounterIncrementedEvent);
        
        notificationCollector.AddNotification(
            new KitWashingFinishedNotification(
                GetKitWashingFinishedContract(kitWashCycle, validationResult.Batch, validationResult.WorkerName, startDate, dateFinished),
                null
                ));
        
        logger.LogInformation("Washing - Kit wash cycle finished with id: {KitWashCycleId} in batch with id: {BatchId}.",
            kitWashCycle.Id, kitWashCycle.BatchId);
        
        return kitWashCycle;
    }

    private static KitWashingFinishedContract GetKitWashingFinishedContract(KitWashCycle kitWashCycle,
        Batch batch, string workerName, DateTimeOffset dateStarted, DateTimeOffset dateFinished, int? globalKitsCount = null)
    {
        var kitsCount = globalKitsCount ?? kitWashCycle.GlobalKitsCount;
        var confirmation = batch.SpecialInformation?
            .Confirmations.FirstOrDefault(x => x.WorkerName == workerName);
        
        var specialInformationContract = confirmation is null ? null :
            new KitFinishedSpecialInformationContract(batch.SpecialInformation!.Id, batch.SpecialInformation.Title, batch.SpecialInformation.Description, batch.SpecialInformation.HasFile, confirmation.WorkerName, confirmation.DateConfirmed);
        
        return new KitWashingFinishedContract(
            kitWashCycle.Id, kitWashCycle.SisterKitId, kitWashCycle.BatchId, batch.SisterBatchId, kitWashCycle.OrderId, batch.PlannedKitsCount,
            batch.WashingMachineCode, batch.LineCode, kitWashCycle.WashingMachineSpeed, (SpeedLevelContract)kitWashCycle.WashingMachineSpeedLevel, 
             batch.OptimalPackagingSpeedOnCurrentMachine, (SpeedLevelContract)batch.OptimalPackagingSpeedOnCurrentMachineLevel, batch.KitCode, batch.KitNumber, batch.KitSapDefinitionCode,
            batch.KitSapDefinitionName, batch.PackagingCode, batch.DefiningPackagingCode, batch.OptimalKitDuration, workerName,
            kitsCount, batch.SapBarcode, kitWashCycle.Efficiency, 
            dateStarted, dateFinished, specialInformationContract);
    }

    private async Task<ErrorOr<ValidationResult>> ValidateSisterBatchAsync(Guid batchId, Batch sisterBatch, string sisterWorkerName, CancellationToken cancellationToken)
    {
        var batch = await GetBatchAsync(batchId, cancellationToken);
        if (batch.IsError)
            return batch.Errors;
        
        var worker = await GetWorkerAsync(batch.Value.LineCode, cancellationToken);
        if (worker.IsError)
            return worker.Errors;
        
        if (batch.Value.IsSpecialInformationConfirmationNeededByWorker(worker.Value.Name) ||
            sisterBatch.IsSpecialInformationConfirmationNeededByWorker(sisterWorkerName))
        {
            logger.LogWarning("Washing - cannot finish sister batch because one is not confirmed by worker. BatchId: {BatchId}, SisterBatchId: {SisterBatchId}.",
                batch.Value.Id, sisterBatch.Id);
            return BatchErrors.ValidationBatchIsNotConfirmedByWorker;
        }
        
        return await ValidateBatchAsync(batch, cancellationToken);
    }

    private async Task<ErrorOr<ValidationResult>> ValidateBatchAsync(ErrorOr<Batch> batch, CancellationToken cancellationToken)
    {
        if (batch.IsError)
            return batch.Errors;
        
        var worker = await GetWorkerAsync(batch.Value.LineCode, cancellationToken);
        if (worker.IsError)
            return worker.Errors;
        
        var result = batch.Value.CanFinishKit(worker.Value.Name);
        
        if (result.IsError)
        {
            logger.LogWarning(
                "Washing - cannot finish kit in batch: {BatchId}. " +
                "Error code: {ErrorCode}, error description: {ErrorDescription}.", 
                batch.Value.Id, result.FirstError.Code, result.FirstError.Description);

            return result.Errors;
        }

        return new ValidationResult(batch.Value, worker.Value.Name);
    }

    private async Task<ErrorOr<Batch>> GetBatchAsync(Guid batchId, CancellationToken cancellationToken)
    {
        var batch = await readOnlyEventStoreRepository.GetByIdAsync<Batch>(batchId, cancellationToken);
        if (batch is null)
        {
            logger.LogError("Washing - Batch with id: {BatchId} was not found. We cannot finish kit for this batch.", batchId);
            return BatchErrors.ValidationBatchDoesNotExist;
        }

        return batch;
    }
    
    private async Task<ErrorOr<Worker>> GetWorkerAsync(string lineCode, CancellationToken cancellationToken)
    {
        var lastEnteredWorkerOnLine = await sender.Send(new GetLastWorkerByLineCodeQuery(lineCode), cancellationToken);
        if (lastEnteredWorkerOnLine.IsError || lastEnteredWorkerOnLine.Value.Worker is null)
        {
            logger.LogWarning("Washing - We cannot finish kit for this batch because there is nobody at the line. LineCode: {LineCode}.", lineCode);
            return BatchErrors.ValidationThereIsNoWorkerAtTheLine;
        }

        return lastEnteredWorkerOnLine.Value.Worker!;
    }

    private record ValidationResult(
        Batch Batch,
        string WorkerName);
}