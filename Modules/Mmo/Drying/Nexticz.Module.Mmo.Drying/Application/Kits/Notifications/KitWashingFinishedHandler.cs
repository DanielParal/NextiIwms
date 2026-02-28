using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Washing.Contracts.Batches.Notifications;
using Nexticz.Module.Mmo.Drying.Application.Kits.Commands.CreateKit;

namespace Nexticz.Module.Mmo.Drying.Application.Kits.Notifications;

internal class KitWashingFinishedHandler(
    ILogger<KitWashingFinishedHandler> logger,
    ISender sender) 
    : INotificationHandler<KitWashingFinishedNotification>
{
    public async Task Handle(KitWashingFinishedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Drying - kit finished notification received. " +
                              "Creating new kit." +
                              "KitId: {KitId}, BatchId: {BatchId}, LineCode: {LineCode}, " +
                              "SisterKitId: {SisterKitId}, SisterBatchId: {SisterBatchId}, SisterLineCode: {SisterLineCode}, " +
                              "StartedDate: {StartedDate}, FinishedDate: {FinishedDate}",
            notification.FinishedKit.KitId, notification.FinishedKit.BatchId, notification.FinishedKit.LineCode, 
            notification.FinishedSisterKit?.KitId, notification.FinishedSisterKit?.BatchId, notification.FinishedSisterKit?.LineCode,
            notification.FinishedKit.WashingStarted, notification.FinishedKit.WashingEnded);

        // We create only one pallet from sister kits that is why we call create kit command only once
        await sender.Send(
            new CreateKitCommand(
                notification.FinishedKit.KitId, notification.FinishedKit.GlobalKitsCount, 
                notification.FinishedKit.BatchId, notification.FinishedKit.LineCode, 
                notification.FinishedKit.KitCode, notification.FinishedKit.WashingEnded), 
            cancellationToken);
    }
}