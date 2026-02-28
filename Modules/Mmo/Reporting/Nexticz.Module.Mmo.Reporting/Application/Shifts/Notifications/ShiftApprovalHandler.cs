using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Reporting.Application.Kits.Notifications;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Notifications;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Orchestrators;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Notifications;

internal class ShiftApprovalHandler(
    IUnapproveShiftOrchestrator unapproveShiftOrchestrator,
    ILogger<ShiftApprovalHandler> logger) : 
    INotificationHandler<ItemsAddedNotification>, 
    INotificationHandler<CommentChangedNotification>, 
    INotificationHandler<LineItemsChangedNotification>,
    INotificationHandler<InactivityCreatedAfterKitFinishedNotification>
{
    
    public async Task Handle(ItemsAddedNotification notification, CancellationToken cancellationToken)
    {
        await ProcessAsync(notification.ShiftId, nameof(ItemsAddedNotification), cancellationToken);
    }

    public async Task Handle(CommentChangedNotification notification, CancellationToken cancellationToken)
    {
        await ProcessAsync(notification.ShiftId, nameof(CommentChangedNotification), cancellationToken);
    }

    public async Task Handle(LineItemsChangedNotification notification, CancellationToken cancellationToken)
    {
        await ProcessAsync(notification.ShiftId, nameof(LineItemsChangedNotification), cancellationToken);
    }

    public async Task Handle(InactivityCreatedAfterKitFinishedNotification notification, CancellationToken cancellationToken)
    {
        foreach (var shiftId in notification.ShiftIds)
        {
            await ProcessAsync(shiftId, nameof(InactivityCreatedAfterKitFinishedNotification), cancellationToken);
        }
    }
    
    private async Task ProcessAsync(Guid shiftId, string notificationType, CancellationToken cancellationToken)
    {
        logger.LogInformation("MMO - reporting - {NotificationType} received. ShiftId: {ShiftId}",
            notificationType, shiftId);
        
        var result = await unapproveShiftOrchestrator.OrchestrateAsync(shiftId, cancellationToken);

        if (result.IsError)
        {
            logger.LogWarning("MMO - reporting - {NotificationType} ended with error. ShiftId: {ShiftId}, " +
                              "ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}",
                notificationType, shiftId,
                result.FirstError.Code, result.FirstError.Description);
            return;
        }
        
        logger.LogInformation("MMO - reporting - {NotificationType} succeeded. ShiftId: {ShiftId}",
            notificationType, shiftId);
    }

    
}