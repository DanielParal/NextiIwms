using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Washing.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Washing.Contracts.Batches.Notifications;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchById;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Application.LastEnteredWorkerOnLines.Queries.GetLastWorkerByLineCode;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Commands.ConfirmSpecialInformation;

internal class ConfirmSpecialInformationCommandHandler(
    ISender sender,
    ILogger<ConfirmSpecialInformationCommandHandler> logger,
    IWashingUnitOfWork unitOfWork,
    IClock clock
    ) : IRequestHandler<ConfirmSpecialInformationCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ConfirmSpecialInformationCommand request, CancellationToken cancellationToken)
    {
        var batch = await sender.Send(new GetBatchByIdQuery(request.BatchId), cancellationToken);
        if (batch.IsError)
        {
            logger.LogWarning("Washing - {ObjectName} with id {Id} not found. We cannot confirm special information.",
                nameof(Batch), request.BatchId);
            return BatchErrors.ValidationBatchDoesNotExist;
        }

        if (batch.Value.SpecialInformation is null)
        {
            logger.LogWarning("Washing - {ObjectName} with id {Id} has no special information. We cannot confirm special information.",
                nameof(Batch), request.BatchId);
            return BatchErrors.ValidationBatchDoesNotHaveSpecialInformation;
        }
        
        var lastEnteredWorkerOnLine = await sender.Send(new GetLastWorkerByLineCodeQuery(batch.Value.LineCode), cancellationToken);
        if (lastEnteredWorkerOnLine.IsError || lastEnteredWorkerOnLine.Value.Worker is null)
        {
            logger.LogWarning("Washing - We cannot finish kit for this batch because there is nobody at the line. LineCode: {LineCode}.", 
                batch.Value.LineCode);
            return BatchErrors.ValidationThereIsNoWorkerAtTheLine;
        }
        
        var dateConfirmed = clock.UtcNowOffset;
        var specialInformationConfirmedEvent = new SpecialInformationConfirmedEvent(batch.Value.Id, batch.Value.SpecialInformation.Id, lastEnteredWorkerOnLine.Value.Worker.Name, dateConfirmed);
        unitOfWork.AppendEvent(batch.Value.Id, specialInformationConfirmedEvent);
        
        logger.LogInformation("Washing - Batch with id: {BatchId} special information with id: {SpecialInformationId} confirmed by worker: {WorkerName} at DateTimeOffset: {ConfirmedDateTimeOffset}.",
            batch.Value.Id, batch.Value.SpecialInformation.Id, lastEnteredWorkerOnLine.Value.Worker.Name, dateConfirmed);
        
        return Result.Success;
    }
}