using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Reporting.Application.Kits.Commands.CreateKit;
using Nexticz.Module.Mmo.Reporting.Application.Shifts;
using Nexticz.Module.Mmo.Washing.Contracts.Batches.Notifications;
using Nexticz.Module.Mmo.Reporting.Application.Kits.Orchestrators;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Orchestrators;

namespace Nexticz.Module.Mmo.Reporting.Application.Kits.Notifications;

internal class KitWashingFinishedHandler(
    ILogger<KitWashingFinishedHandler> logger,
    IShiftCreationOrchestrator shiftCreationOrchestrator,
    ICreateItemsAfterKitFinishedOrchestrator afterKitFinishedOrchestrator) 
    : INotificationHandler<KitWashingFinishedNotification>
{
    public async Task Handle(KitWashingFinishedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Reporting - kit washing finished notification received. " +
                              "Creating new kit(s)." +
                              "KitId: {KitId}, BatchId: {BatchId}, LineCode: {LineCode}, " +
                              "SisterKitId: {SisterKitId}, SisterBatchId: {SisterBatchId}, SisterLineCode: {SisterLineCode}, " +
                              "GlobalKitsCount: {GlobalKitsCount}, StartedDate: {StartedDate}, FinishedDate: {FinishedDate}",
            notification.FinishedKit.KitId, notification.FinishedKit.BatchId, notification.FinishedKit.LineCode, 
            notification.FinishedSisterKit?.KitId, notification.FinishedSisterKit?.BatchId, notification.FinishedSisterKit?.LineCode,
            notification.FinishedKit.GlobalKitsCount, notification.FinishedKit.WashingStarted, notification.FinishedKit.WashingEnded);
        
        var shiftId = await shiftCreationOrchestrator.EnsureCurrentShiftAsync(cancellationToken);
        
        await afterKitFinishedOrchestrator.OrchestrateAsync(shiftId, notification.FinishedKit, notification.FinishedSisterKit, cancellationToken);
    }
}