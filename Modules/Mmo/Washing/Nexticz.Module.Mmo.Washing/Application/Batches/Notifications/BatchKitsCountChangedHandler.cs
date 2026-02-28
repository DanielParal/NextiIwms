using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines.Notifications;
using Nexticz.Module.Mmo.Washing.Application.Batches.Commands.ChangePlannedKitsCount;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Notifications;

internal class BatchKitsCountChangedHandler(
    ILogger<BatchKitsCountChangedHandler> logger,
    ISender sender) : INotificationHandler<BatchKitsCountChangedNotification>
{
    public async Task Handle(BatchKitsCountChangedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Washing - batch kits count changed notification received. " +
                              "BatchId: {BatchId}, SisterBatchId: {SisterBatchId}, " +
                              "NewKitsCount: {NewKitsCount}.",
            notification.BatchId, notification.SisterBatchId, notification.NewKitsCount);

        var result =
            await sender.Send(new ChangePlannedKitsCountCommand(notification.BatchId, notification.NewKitsCount), cancellationToken);
        
        if (!result.IsError && notification.SisterBatchId is not null)
            await sender.Send(new ChangePlannedKitsCountCommand(notification.SisterBatchId.Value, notification.NewKitsCount), cancellationToken);
    }
}