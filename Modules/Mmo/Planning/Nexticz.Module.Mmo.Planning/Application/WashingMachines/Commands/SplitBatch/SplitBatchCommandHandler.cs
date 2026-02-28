using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Orchestrators.SplitBatch;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetBatchByIdAndLineCode;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachineByLineQueueCode;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.SplitBatch;

internal class SplitBatchCommandHandler(
    ISplitBatchOrchestrator splitBatchOrchestrator,
    ISender sender,
    ILogger<SplitBatchCommandHandler> logger) 
    : IRequestHandler<SplitBatchCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(SplitBatchCommand request, CancellationToken cancellationToken)
    {
        var upperLineQueueCode = request.LineQueueCode.ToUpperInvariant();
        var batch = await sender.Send(new GetBatchByIdAndLineCodeQuery(request.BatchId, upperLineQueueCode), cancellationToken);
        if (batch.IsError)
            return batch.Errors;

        if (!batch.Value.HasSisterBatch)
        {
            logger.LogInformation("Planning - batch with id: {BatchId} is gonna split on the line with code {LineCode}. " +
                                  "Count to change: {CountToChange}, new kits count: {NewKitsCount}", 
                batch.Value.Id, upperLineQueueCode, request.CountToChange, request.KitsCountToCreate);
            
            var resultSplitSingleBatch = await splitBatchOrchestrator.OrchestrateSingleBatchAsync(
                    batch.Value, upperLineQueueCode, request.CountToChange, request.KitsCountToCreate, cancellationToken);
            
            if (resultSplitSingleBatch.IsError)
                return resultSplitSingleBatch.Errors;
            
            logger.LogInformation("Planning - batch with id: {BatchId} is split on the line with code {LineCode}. " +
                                  "Count to change: {CountToChange}, new kits count: {NewKitsCount}", 
                batch.Value.Id, upperLineQueueCode, request.CountToChange, request.KitsCountToCreate);

            return Result.Success;
        }
        
        var machineMachine = await sender.Send(new GetWashingMachineByLineQueueCodeQuery(upperLineQueueCode), cancellationToken);
        if (machineMachine.IsError)
            return machineMachine.Errors;
        
        var sisterLineQueue = machineMachine.Value.LineQueues.First(x => x.WashingMachineLineCode != upperLineQueueCode);
        var sisterBatch = sisterLineQueue.Batches.First(x => x.Id == batch.Value.SisterBatchId);
        
        logger.LogInformation("Planning - batch with id: {BatchId} is gonna split on the line with code {LineCode} " +
                              "and sister batch with id: {SisterBatchId} is gonna split on the line with code {SisterLineCode}. " +
                              "Count to change: {CountToChange}, new kits count: {NewKitsCount}", 
            batch.Value.Id, upperLineQueueCode, sisterBatch.Id, sisterLineQueue.WashingMachineLineCode, request.CountToChange, request.KitsCountToCreate);
        
        var resultSplitSisterBatch = await splitBatchOrchestrator.OrchestrateSisterBatchesAsync(
            batch.Value, sisterBatch, upperLineQueueCode, request.CountToChange, request.KitsCountToCreate, cancellationToken);
        
        if (resultSplitSisterBatch.IsError)
            return resultSplitSisterBatch.Errors;
            
        logger.LogInformation("Planning - batch with id: {BatchId} is split on the line with code {LineCode} " +
                              "and sister batch with id: {SisterBatchId} is split on the line with code {SisterLineCode}. " +
                              "Count to change: {CountToChange}, new kits count: {NewKitsCount}", 
            batch.Value.Id, upperLineQueueCode, sisterBatch.Id, sisterLineQueue.WashingMachineLineCode, request.CountToChange, request.KitsCountToCreate);

        return Result.Success;
    }
}