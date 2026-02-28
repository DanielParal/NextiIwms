using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.SplitBatch;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Orchestrators.ActivateBatch;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetBatchByIdAndLineCode;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachineByLineQueueCode;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.ActivateBatch;

internal class ActivateBatchCommandHandler(
    IActivateBatchOrchestrator activateBatchOrchestrator,
    ISender sender,
    ILogger<SplitBatchCommandHandler> logger)  : IRequestHandler<ActivateBatchCommand, ErrorOr<ActivateBatchResponse>>
{
    public async Task<ErrorOr<ActivateBatchResponse>> Handle(ActivateBatchCommand request, CancellationToken cancellationToken)
    {
        var upperLineQueueCode = request.LineQueueCode.ToUpperInvariant();
        
        var washingMachine = await GetWashingMachineByLineQueueCodeAsync(upperLineQueueCode, cancellationToken);
        if(washingMachine.IsError)
            return washingMachine.Errors;
        
        var batch = await sender.Send(new GetBatchByIdAndLineCodeQuery(request.BatchId, upperLineQueueCode), cancellationToken);
        if (batch.IsError)
            return batch.Errors;
        
        var currentBatchesInWashing = GetCurrentBatchesInWashing(washingMachine.Value, batch.Value.HasSisterBatch, upperLineQueueCode);
        
        logger.LogInformation("Planning - batch with id: {BatchId} is gonna activate on the line with code {LineCode}. " +
                              "Current batch is gonna finish: {CurrentBatchId}, Current batch in other line is: {CurrentBatchInOtherLine}.", 
            batch.Value.Id, upperLineQueueCode, currentBatchesInWashing.BatchInCurrentLine?.Id, currentBatchesInWashing.BatchInOtherLine?.Id);
            
        var resultActivateBatch = await activateBatchOrchestrator.OrchestrateAsync(
            batch.Value, washingMachine.Value.Code,  upperLineQueueCode, currentBatchesInWashing.BatchInCurrentLine, currentBatchesInWashing.CurrentLineCode,
            currentBatchesInWashing.BatchInOtherLine, currentBatchesInWashing.OtherLineCode,
            cancellationToken);
            
        if (resultActivateBatch.IsError)
            return resultActivateBatch.Errors;
            
        logger.LogInformation("Planning - batch with id: {BatchId} is activated on the line with code {LineCode}. " +
                              "Current batch is finished: {CurrentBatchId}, Current batch in other line is: {CurrentBatchInOtherLine}.", 
            batch.Value.Id, upperLineQueueCode, currentBatchesInWashing.BatchInCurrentLine?.Id, currentBatchesInWashing.BatchInOtherLine?.Id);

        return resultActivateBatch.Value;
    }
    
    private async Task<ErrorOr<WashingMachine>> GetWashingMachineByLineQueueCodeAsync(string lineQueueCode, CancellationToken cancellationToken)
    {
        var washingMachine = 
            await sender.Send(
                new GetWashingMachineByLineQueueCodeQuery(lineQueueCode), cancellationToken);

        if (washingMachine.IsError)
        {
            logger.LogWarning("Object {ObjectName} with code: {Code} does not exist. We cannot finish batch.",
                nameof(LineQueue), lineQueueCode);
            return WashingMachineErrors.ValidationLineQueueDoesNotExist;
        }
        
        return washingMachine.Value;
    }
    
    private static CurrentBatchesInWashing GetCurrentBatchesInWashing(WashingMachine washingMachine, bool isNewSisterBatch, string upperLineQueueCode)
    {
        var currentBatchInWashing = washingMachine
            .LineQueues
            .First(x => x.WashingMachineLineCode == upperLineQueueCode)
            .Batches
            .FirstOrDefault(x => x.Status == BatchStatus.Washing);

        // we are activating single batch - we need just batch from current line
        if (!isNewSisterBatch)
            return new CurrentBatchesInWashing(currentBatchInWashing, upperLineQueueCode, null, null);
        
        // we are activating new sister batch - current batch in washing is sister so it is gonna be finished by one command
        if (currentBatchInWashing is not null && currentBatchInWashing.HasSisterBatch)
            return new CurrentBatchesInWashing(currentBatchInWashing, upperLineQueueCode, null, null);
        
        var otherLine = 
            washingMachine.LineQueues
                .FirstOrDefault(x => x.WashingMachineLineCode != upperLineQueueCode);
        
        // this should not happen - current washing machine does not have sister line - so we are gonna finish only current batch
        if (otherLine is null)
            return new CurrentBatchesInWashing(currentBatchInWashing, upperLineQueueCode, null, null);

        var otherBatchInWashing = otherLine.Batches.FirstOrDefault(x => x.Status == BatchStatus.Washing);
        
        // return two single batches which are currently in washing
        return new CurrentBatchesInWashing(currentBatchInWashing, upperLineQueueCode, otherBatchInWashing, otherLine.WashingMachineLineCode);
    }

    private record CurrentBatchesInWashing(Batch? BatchInCurrentLine, string CurrentLineCode, Batch? BatchInOtherLine, string? OtherLineCode);
}