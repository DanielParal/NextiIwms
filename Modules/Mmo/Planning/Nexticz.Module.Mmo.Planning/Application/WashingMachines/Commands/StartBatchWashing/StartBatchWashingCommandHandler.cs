using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachineByLineQueueCode;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.StartBatchWashing;

internal class StartBatchWashingCommandHandler(
    ISender sender,
    ILogger<StartBatchWashingCommandHandler> logger,
    IPlanningUnitOfWork unitOfWork) : IRequestHandler<StartBatchWashingCommand, ErrorOr<(BatchActivatedResponse BatchStarted, BatchActivatedResponse? SisterBatchStarted)>>
{
    public async Task<ErrorOr<(BatchActivatedResponse BatchStarted, BatchActivatedResponse? SisterBatchStarted)>> Handle(StartBatchWashingCommand request, CancellationToken cancellationToken)
    {
        var upperLineQueueCode = request.LineQueueCode.ToUpperInvariant();
        // 1. get washing machine by line queue code
        var washingMachine = await GetWashingMachineByLineQueueCodeAsync(upperLineQueueCode, cancellationToken);
        if(washingMachine.IsError)
            return washingMachine.Errors;
        
        // 2. verify batch id is present in queue
        var batch = GetBatchFromQueue(washingMachine.Value, request.BatchId, upperLineQueueCode);
        if (batch.IsError)
            return batch.Errors;

        var dateStarted = request.DateActivatedBatch;
        if (!batch.Value.HasSisterBatch)
        {
            var resultStartSingleBatch = 
                StartSingleBatch(washingMachine.Value, upperLineQueueCode, batch.Value, dateStarted);
            if (resultStartSingleBatch.IsError)
                return resultStartSingleBatch.Errors;
            
            logger.LogInformation("Planning - Batch with id: {BatchId} started washing on washing machine line with code: {LineCode}.", 
                batch.Value.Id, upperLineQueueCode);
            return (resultStartSingleBatch.Value, null);
        }
        
        var resultStartSisterBatch = 
            StartSisterBatch(washingMachine.Value, upperLineQueueCode, batch.Value, dateStarted);
        if (resultStartSisterBatch.IsError)
            return resultStartSisterBatch.Errors;
            
        logger.LogInformation("Planning - Sister batch with id: {BatchId} and sister batch id: {SisterBatchId} started washing on washing machine with code: {WashingMachineCode}.", 
            batch.Value.Id, batch.Value.SisterBatchId, washingMachine.Value.Code);
        return resultStartSisterBatch.Value;
    }

    private ErrorOr<BatchActivatedResponse> StartSingleBatch(
        WashingMachine washingMachine, string upperLineQueueCode, Batch batch, DateTimeOffset dateStarted)
    {
        var validation = washingMachine.StartBatchWashing(batch.Id, dateStarted);
        if (validation.IsError)
        {
            logger.LogWarning("Planning - Cannot start washing batch in line queue with code: {Code}. " +
                            "Error code: {ErrorCode}, error description: {ErrorDescription}." +
                            "KitCode: {KitCode}, PackagingCode: {PackagingCode}, CurrentKitsCount: {KitsCount}, " +
                            "OptimalKitDuration: {OptimalKitDuration}.", 
                upperLineQueueCode, validation.FirstError.Code, validation.FirstError.Description,
                batch.KitCode, batch.PackagingCode, batch.KitsCount, batch.OptimalKitDuration);
            return validation.Errors;
        }
        
        var batchStartedWashingEvent = new BatchWashingStartedEvent(
            batch.Id, washingMachine.Code, upperLineQueueCode, dateStarted);
        unitOfWork.AppendEvent(washingMachine.Id, batchStartedWashingEvent);
        
        return new BatchActivatedResponse(BatchContractFactory.Create(batch),
            washingMachine.Code, upperLineQueueCode, dateStarted);
    }
    
    private ErrorOr<(BatchActivatedResponse BatchStarted, BatchActivatedResponse SisterBatchStarted)> StartSisterBatch(
        WashingMachine washingMachine, string upperLineQueueCode, Batch batch, DateTimeOffset dateStarted)
    {
        if (washingMachine.IsOneLineMachine)
        {
            logger.LogWarning("Planning - Cannot start washing sister batch with id {BatchId} in washing machine with code: {WashingMachine}. " +
                              "Washing machine has only one line.", 
                batch.Id, washingMachine.Code);
            return WashingMachineErrors.ValidationSingleLineWashingMachine;
        }
        
        var upperSisterLineQueueCode = 
            washingMachine.LineQueues
                .First(x => x.WashingMachineLineCode != upperLineQueueCode)
                .WashingMachineLineCode.ToUpperInvariant();
        
        var sisterBatch = GetBatchFromQueue(washingMachine, (Guid)batch.SisterBatchId!, upperSisterLineQueueCode);
        if (sisterBatch.IsError)
            return sisterBatch.Errors;
        
        var validation = washingMachine.StartSisterBatchWashing(batch.Id, sisterBatch.Value.Id, dateStarted);
        if (validation.IsError)
        {
            logger.LogWarning("Planning - Cannot start washing sister batches in line queues with code: {LineQueueCode}, {SisterLineQueueCode}. " +
                              "Error code: {ErrorCode}, error description: {ErrorDescription}." +
                              "BatchId: {BatchId}, SisterBatchId: {SisterBatchId}.", 
                upperLineQueueCode, upperSisterLineQueueCode, validation.FirstError.Code, validation.FirstError.Description,
                batch.Id, sisterBatch.Value.Id);
            return validation.Errors;
        }
        
        var sisterBatchWashingStartedEvent = 
            new SisterBatchWashingStartedEvent(
                batch.Id, sisterBatch.Value.Id, washingMachine.Code, upperLineQueueCode, upperSisterLineQueueCode, dateStarted);
        
        unitOfWork.AppendEvent(washingMachine.Id, sisterBatchWashingStartedEvent);
        
        var batchActivated = new BatchActivatedResponse(BatchContractFactory.Create(batch), washingMachine.Code, upperLineQueueCode, dateStarted);
        var sisterBatchActivated = new BatchActivatedResponse(BatchContractFactory.Create(sisterBatch.Value),
            washingMachine.Code, upperSisterLineQueueCode, dateStarted);
        return (batchActivated, sisterBatchActivated);
    }
    
    private async Task<ErrorOr<WashingMachine>> GetWashingMachineByLineQueueCodeAsync(string lineQueueCode, CancellationToken cancellationToken)
    {
        var washingMachine = 
            await sender.Send(
                new GetWashingMachineByLineQueueCodeQuery(lineQueueCode), cancellationToken);

        if (washingMachine.IsError)
        {
            logger.LogWarning("Planning - Object {ObjectName} with code: {Code} does not exist. We cannot start batch.",
                nameof(LineQueue), lineQueueCode);
            return WashingMachineErrors.ValidationLineQueueDoesNotExist;
        }
        
        return washingMachine.Value;
    }
    
    private ErrorOr<Batch> GetBatchFromQueue(WashingMachine washingMachine, Guid batchId, string lineQueueCode)
    {
        var batch = washingMachine
            .LineQueues
            .First(x => x.WashingMachineLineCode == lineQueueCode)
            .Batches
            .FirstOrDefault(x => x.Id == batchId);

        if (batch is null)
        {
            logger.LogWarning("Planning - Object {ObjectName} with id: {Id} does not exist in the queue: {QueueId}. We cannot start batch.",
                nameof(Batch), batchId, lineQueueCode);
            return WashingMachineErrors.ValidationBatchIsNotPresentInTheQueue;
        }
        
        return batch;
    }
}