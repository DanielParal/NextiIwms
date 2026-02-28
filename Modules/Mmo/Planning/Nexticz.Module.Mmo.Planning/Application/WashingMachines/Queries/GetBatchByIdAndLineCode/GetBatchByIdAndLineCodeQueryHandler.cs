using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachineByLineQueueCode;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetBatchByIdAndLineCode;

internal class GetBatchByIdAndLineCodeQueryHandler(
    ISender sender,
    ILogger<GetBatchByIdAndLineCodeQueryHandler> logger
    ) : IRequestHandler<GetBatchByIdAndLineCodeQuery, ErrorOr<Batch>>
{
    public async Task<ErrorOr<Batch>> Handle(GetBatchByIdAndLineCodeQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.LineCode))
        {
            logger.LogWarning("Line code is not filled in. We cannot get batch by id and line code.");
            return WashingMachineErrors.ValidationLineCodeIsRequired;
        }
        
        var upperLineQueueCode = request.LineCode.ToUpperInvariant();
        
        // 1. get washing machine by line queue code
        var washingMachine = await GetWashingMachineByLineQueueCodeAsync(upperLineQueueCode, cancellationToken);
        if(washingMachine.IsError)
            return washingMachine.Errors;
        
        // 2. verify batch id is present in queue
        var batch = GetBatchFromQueue(washingMachine.Value, request.BatchId, upperLineQueueCode);
        if (batch.IsError)
            return batch.Errors;
        
        return batch.Value;
    }
    
    private async Task<ErrorOr<WashingMachine>> GetWashingMachineByLineQueueCodeAsync(string lineQueueCode, CancellationToken cancellationToken)
    {
        var washingMachine = 
            await sender.Send(
                new GetWashingMachineByLineQueueCodeQuery(lineQueueCode), cancellationToken);

        if (washingMachine.IsError)
        {
            logger.LogWarning("Object {ObjectName} with code: {Code} does not exist. Line code is not filled in. We cannot get batch by id and line code.",
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
            logger.LogWarning("Object {ObjectName} with id: {Id} does not exist in the queue: {QueueId}. Line code is not filled in. We cannot get batch by id and line code.",
                nameof(Batch), batchId, lineQueueCode);
            return WashingMachineErrors.ValidationBatchIsNotPresentInTheQueue;
        }
        
        return batch;
    }
}