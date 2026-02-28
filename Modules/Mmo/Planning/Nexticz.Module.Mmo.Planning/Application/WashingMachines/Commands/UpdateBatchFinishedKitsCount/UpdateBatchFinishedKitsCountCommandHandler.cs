using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachineByLineQueueCode;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.UpdateBatchFinishedKitsCount;

internal class UpdateBatchFinishedKitsCountCommandHandler(
    ISender sender,
    ILogger<UpdateBatchFinishedKitsCountCommandHandler> logger,
    IPlanningUnitOfWork planningUnitOfWork
    ) : IRequestHandler<UpdateBatchFinishedKitsCountCommand, ErrorOr<Batch>>
{
    public async Task<ErrorOr<Batch>> Handle(UpdateBatchFinishedKitsCountCommand request, CancellationToken cancellationToken)
    {
        var upperLineQueueCode = request.LineQueueCode.ToUpperInvariant();

        var washingMachine = await GetWashingMachineByLineQueueCodeAsync(upperLineQueueCode, cancellationToken);
        if(washingMachine.IsError)
            return washingMachine.Errors;
        
        var batch = GetBatchFromQueue(washingMachine.Value, request.BatchId, upperLineQueueCode);
        if (batch.IsError)
            return batch.Errors;

        batch.Value.FinishKit(request.DateFinished);
        
        var batchKitFinishedEvent = new BatchKitFinishedEvent(request.BatchId, washingMachine.Value.Code, upperLineQueueCode, request.DateFinished);
        planningUnitOfWork
            .AppendEvent(washingMachine.Value.Id, batchKitFinishedEvent);
        
        logger.LogInformation("Planning - Batch with id: {BatchId} updated with finished kits count. New finished kits count: {Count}. LineCode: {LineCode}.",
            batch.Value.Id, batch.Value.KitsFinished, upperLineQueueCode);
        
        return batch;
    }
    
    private async Task<ErrorOr<WashingMachine>> GetWashingMachineByLineQueueCodeAsync(string lineQueueCode, CancellationToken cancellationToken)
    {
        var washingMachine = 
            await sender.Send(
                new GetWashingMachineByLineQueueCodeQuery(lineQueueCode), cancellationToken);

        if (washingMachine.IsError)
        {
            logger.LogWarning("Object {ObjectName} with code: {Code} does not exist. We cannot remove batch from queue.",
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
            logger.LogWarning("Object {ObjectName} with id: {Id} does not exist in the queue: {QueueId}. We cannot remove batch from queue.",
                nameof(Batch), batchId, lineQueueCode);
            return WashingMachineErrors.ValidationBatchIsNotPresentInTheQueue;
        }
        
        return batch;
    }
}