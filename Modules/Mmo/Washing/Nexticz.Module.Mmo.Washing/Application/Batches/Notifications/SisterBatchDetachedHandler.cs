using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines.Notifications;
using Nexticz.Module.Mmo.Washing.Application.Batches.Commands.DetachBatch;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Notifications;

internal class SisterBatchDetachedHandler(
    ILogger<SisterBatchDetachedHandler> logger,
    ISender sender) : INotificationHandler<SisterBatchDetachedNotification>
{
    public async Task Handle(SisterBatchDetachedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Washing - Sister batch detached notification received. " +
                              "BatchId: {BatchId}, SisterBatchId: {SisterBatchId}, " +
                              "LineCode: {LineCode}, SisterLineCode: {SisterLineCode}.",
            notification.BatchId, notification.SisterBatchId, notification.LineCode, notification.LineCode);
        
        var result = await sender.Send(
            new DetachBatchCommand(notification.BatchId),
            cancellationToken);

        if (!result.IsError)
        {
            await sender.Send(
                new DetachBatchCommand(notification.SisterBatchId),
                cancellationToken);
        }
    }
}