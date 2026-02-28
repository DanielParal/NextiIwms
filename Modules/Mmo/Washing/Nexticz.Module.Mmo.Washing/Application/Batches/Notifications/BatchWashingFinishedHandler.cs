using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines.Notifications;
using Nexticz.Module.Mmo.Washing.Application.Batches.Commands.FinishBatchWashing;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Notifications;

internal class BatchWashingFinishedHandler (
    ILogger<BatchWashingFinishedHandler> logger,
    ISender sender) 
    : INotificationHandler<BatchWashingFinishedNotification>
{
    public async Task Handle(BatchWashingFinishedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Washing - batch washing finished notification received. " +
                              "BatchId: {BatchId}, SisterBatchId: {SisterBatchId}, FinishedDate: {FinishedDate}",
            notification.BatchId, notification.SisterBatchId, notification.DateFinished);

        var result = await sender.Send(
            new FinishBatchWashingCommand(notification.BatchId, notification.DateFinished),
            cancellationToken);

        if (!result.IsError && notification.SisterBatchId is not null)
        {
            await sender.Send(
                new FinishBatchWashingCommand(notification.SisterBatchId.Value, notification.DateFinished),
                cancellationToken);
        }
    }
}