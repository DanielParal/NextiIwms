using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Drying.Contracts.Kits.Notifications;
using Nexticz.Module.Mmo.Reporting.Application.DriedKits.Commands.CreateDriedKit;
using Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.DriedKits.Notifications;

internal class KitDryingFinishedHandler(
    ILogger<KitDryingFinishedHandler> logger,
    ISender sender
    ) : INotificationHandler<KitDryingFinishedNotification>
{
    public async Task Handle(KitDryingFinishedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Reporting - Kit drying finished notification received. " +
                              "Creating new kit in reporting. " +
                              "KitIdFromDrying: {KitIdFromDrying}, BatchId: {BatchId}, FinishedDate: {FinishedDate}, LineCode: {LineCode}",
            notification.Kit.KitId, notification.Kit.BatchId, notification.Kit.DryingEnded, notification.Kit.LineCode);
        
        var result = await sender.Send(
            new CreateDriedKitCommand(
                notification.Kit.KitId, notification.Kit.BatchId, notification.Kit.CompletedKitsCount, notification.Kit.KitCode, 
                notification.Kit.LineCode, notification.Kit.ExpectedDryingTime, notification.Kit.DryingStarted, 
                notification.Kit.DryingEnded, (DriedKitDestination)notification.Kit.Destination, notification.Kit.TransferredToDryingSectionAt), 
            cancellationToken);

        if (result.IsError)
        {
            logger.LogWarning("Reporting - Create dried kit command from notification failed. " +
                              "Error code: {ErrorCode}, ErrorDescription: {ErrorDescription}, KitId: {KitId}",
                result.FirstError.Code, result.FirstError.Description, notification.Kit.KitId);
        }
    }
}