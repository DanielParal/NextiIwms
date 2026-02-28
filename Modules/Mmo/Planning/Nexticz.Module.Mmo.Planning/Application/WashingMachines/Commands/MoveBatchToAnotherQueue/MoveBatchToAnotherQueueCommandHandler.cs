using ErrorOr;
using JasperFx.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Orchestrators.MoveBatchToAnotherQueue;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetBatchByIdAndLineCode;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachineByLineQueueCode;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.MoveBatchToAnotherQueue;

internal class MoveBatchToAnotherQueueCommandHandler(
    IMoveBatchToAnotherQueueOrchestrator moveBatchToAnotherQueueOrchestrator,
    ISender sender,
    ILogger<MoveBatchToAnotherQueueCommandHandler> logger) : IRequestHandler<MoveBatchToAnotherQueueCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(MoveBatchToAnotherQueueCommand request, CancellationToken cancellationToken)
    {
        var upperCurrentLineQueueCode = request.CurrentLineQueueCode.ToUpperInvariant();
        var upperNewLineQueueCode = request.NewLineQueueCode.ToUpperInvariant();
        
        var batch = await sender.Send(new GetBatchByIdAndLineCodeQuery(request.BatchId, upperCurrentLineQueueCode), cancellationToken);
        if (batch.IsError)
            return batch.Errors;

        if (!batch.Value.HasSisterBatch)
        {
            return await MoveSingleBatchAsync(batch.Value, upperCurrentLineQueueCode, upperNewLineQueueCode, cancellationToken);
        }
        
        return await MoveSisterBatchAsync(batch.Value, upperCurrentLineQueueCode, upperNewLineQueueCode, cancellationToken);
    }

    private async Task<ErrorOr<Success>> MoveSisterBatchAsync(Batch batch, string upperCurrentLineQueueCode,
        string upperNewLineQueueCode, CancellationToken cancellationToken)
    {
        var washingMachine = await sender.Send(new GetWashingMachineByLineQueueCodeQuery(upperCurrentLineQueueCode), cancellationToken);
        if (washingMachine.IsError)
            return washingMachine.Errors;
        
        var sisterLineQueue = washingMachine.Value.LineQueues.First(x => x.WashingMachineLineCode != upperCurrentLineQueueCode);
        var sisterBatch = sisterLineQueue.Batches.First(x => x.Id == batch.SisterBatchId);
        
        logger.LogInformation("Planning - batch with id: {BatchId} with sister batch with id: {SisterBatchId} are gonna move from line {CurrentLineCode} to new line {NewLineCode}.", 
            batch.Id, batch.SisterBatchId, upperCurrentLineQueueCode, upperNewLineQueueCode);
        
        var resultMoveSisterBatch = await moveBatchToAnotherQueueOrchestrator.OrchestrateSisterBatchesAsync(
            batch, sisterBatch, upperCurrentLineQueueCode, upperNewLineQueueCode, cancellationToken);
        
        if (resultMoveSisterBatch.IsError)
            return resultMoveSisterBatch.Errors;
            
        logger.LogInformation("Planning - batch with id: {BatchId} with sister batch with id: {SisterBatchId} are moved from line {CurrentLineCode} to new line {NewLineCode}.", 
            batch.Id, batch.SisterBatchId, upperCurrentLineQueueCode, upperNewLineQueueCode);

        return Result.Success;
    }

    private async Task<ErrorOr<Success>> MoveSingleBatchAsync(Batch batch, string upperCurrentLineQueueCode,
        string upperNewLineQueueCode, CancellationToken cancellationToken)
    {
        logger.LogInformation("Planning - batch with id: {BatchId} is gonna move from line {CurrentLineCode} to new line {NewLineCode}.", 
            batch.Id, upperCurrentLineQueueCode, upperNewLineQueueCode);
            
        var resultMoveSingleBatch = await moveBatchToAnotherQueueOrchestrator.OrchestrateSingleBatchAsync(
            batch, upperCurrentLineQueueCode, upperNewLineQueueCode, cancellationToken);
            
        if (resultMoveSingleBatch.IsError)
            return resultMoveSingleBatch.Errors;
            
        logger.LogInformation("Planning - batch with id: {BatchId} is moved from line {CurrentLineCode} to new line {NewLineCode}.", 
            batch.Id, upperCurrentLineQueueCode, upperNewLineQueueCode);

        return Result.Success;
    }
}