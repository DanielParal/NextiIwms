using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Washing.Contracts.Batches.Notifications;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.UpdateBatchFinishedKitsCount;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Notifications;

internal class KitWashCycleFinishedHandler(
    ILogger<KitWashCycleFinishedHandler> logger,
    ISender sender) 
    : INotificationHandler<KitWashingFinishedNotification>
{
    public async Task Handle(KitWashingFinishedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Planning - kit wash cycle finished notification received. " +
                              "Updating count of finished kits." +
                              "KitWashCycleId: {KitId}, BatchId: {BatchId}, LineCode: {LineCode}, " +
                              "SisterKitWashCycleId: {SisterKitId}, SisterBatchId: {SisterBatchId}, SisterLineCode: {SisterLineCode}, " +
                              "StartedDate: {StartedDate}, FinishedDate: {FinishedDate}",
            notification.FinishedKit.KitId, notification.FinishedKit.BatchId, notification.FinishedKit.LineCode, 
            notification.FinishedSisterKit?.KitId, notification.FinishedSisterKit?.BatchId, notification.FinishedSisterKit?.LineCode,
            notification.FinishedKit.WashingStarted, notification.FinishedKit.WashingEnded);

        var result = await sender.Send(
            new UpdateBatchFinishedKitsCountCommand(
                notification.FinishedKit.BatchId, notification.FinishedKit.LineCode, notification.FinishedKit.WashingEnded),
            cancellationToken);

        if (result.IsError || notification.FinishedSisterKit is null)
            return;
        
        await sender.Send(
            new UpdateBatchFinishedKitsCountCommand(
                notification.FinishedSisterKit.BatchId, notification.FinishedSisterKit.LineCode, notification.FinishedSisterKit.WashingEnded),
            cancellationToken);
    }
}