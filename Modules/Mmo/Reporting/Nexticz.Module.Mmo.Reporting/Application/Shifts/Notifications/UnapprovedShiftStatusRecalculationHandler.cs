using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Reporting.Application.Kits.Notifications;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Notifications;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemsByShiftId;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Commands.ChangeShiftStatus;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetUnapprovedShiftStatusViewByShiftId;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Notifications;

internal class UnapprovedShiftStatusRecalculationHandler(
    ISender sender,
    ILogger<UnapprovedShiftStatusRecalculationHandler> logger) : 
    INotificationHandler<ItemsAddedNotification>, 
    INotificationHandler<InactivityCreatedAfterKitFinishedNotification>,
    INotificationHandler<LineItemsChangedNotification>,
    INotificationHandler<CommentChangedNotification>
{
    public async Task Handle(ItemsAddedNotification notification, CancellationToken cancellationToken)
    {
        await ProcessRecalculationAsync(notification.ShiftId, nameof(ItemsAddedNotification), cancellationToken);
    }

    public async Task Handle(InactivityCreatedAfterKitFinishedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("[MMO] [Start Foreach] [UnapprovedShiftStatusRecalculationHandler] from event: {event}. ShiftIds: {@ShiftIds}", nameof(InactivityCreatedAfterKitFinishedNotification), notification.ShiftIds);

        foreach (var shiftId in notification.ShiftIds)
        {
            await ProcessRecalculationAsync(shiftId, nameof(InactivityCreatedAfterKitFinishedNotification), cancellationToken);
        }
        
        logger.LogInformation("[MMO] [End Foreach] [UnapprovedShiftStatusRecalculationHandler] from event: {event}. ShiftIds: {@ShiftIds}", nameof(InactivityCreatedAfterKitFinishedNotification), notification.ShiftIds);
    }

    public async Task Handle(CommentChangedNotification notification, CancellationToken cancellationToken)
    {
        await ProcessRecalculationAsync(notification.ShiftId, nameof(CommentChangedNotification), cancellationToken);
    }
    
    
    public async Task Handle(LineItemsChangedNotification notification, CancellationToken cancellationToken)
    {
        await ProcessRecalculationAsync(notification.ShiftId, nameof(LineItemsChangedNotification), cancellationToken);
    }

    private async Task ProcessRecalculationAsync(Guid shiftId, string eventName, CancellationToken cancellationToken)
    {
        logger.LogInformation("[MMO] [Start] [UnapprovedShiftStatusRecalculationHandler] from event: {event}. ShiftId: {ShiftId}", eventName, shiftId);
        
        var currentShiftStatus = await GetCurrentShiftStatusAsync(shiftId, cancellationToken);
        if (currentShiftStatus is null)
        {
            logger.LogInformation("[MMO] [End] [UnapprovedShiftStatusRecalculationHandler] shift is not in unapproved view. Status does not change. ShiftId: {ShiftId}", shiftId);
            return;
        }
        
        var lineItemsForReview = await GetLineItemsForReviewAsync(shiftId, cancellationToken);
        var newShiftStatus = GetShiftStatus(lineItemsForReview);

        if (currentShiftStatus.Status == newShiftStatus)
        {
            logger.LogInformation("[MMO] [End] [UnapprovedShiftStatusRecalculationHandler] status does not change. CurrentStatus: {CurrentStatus}, ShiftId: {ShiftId}", currentShiftStatus.Status, shiftId);
            return;
        }

        await sender.Send(new ChangeShiftStatusCommand(shiftId, currentShiftStatus.Status, newShiftStatus),
            cancellationToken);
        
        logger.LogInformation("[MMO] [End] [UnapprovedShiftStatusRecalculationHandler] status changed from: {CurrentShiftStatus}, to: {ToShiftStatus}. ShiftId: {ShiftId}", 
            currentShiftStatus.Status, newShiftStatus, shiftId);
    }

    private async Task<LineItemView[]> GetLineItemsForReviewAsync(Guid shiftId, CancellationToken cancellationToken)
    {
        var lineItems = await sender.Send(new GetLineItemsByShiftIdQuery(shiftId), cancellationToken);
        var lineItemsForReview = LineItemForReviewFilter.FilterLineItemsForReview(lineItems.ToArray());
        return lineItemsForReview;
    }

    private async Task<UnapprovedShiftStatusView?> GetCurrentShiftStatusAsync(Guid shiftId, CancellationToken cancellationToken)
    {
        var currentUnapprovedShiftStatusView = await sender.Send(new GetUnapprovedShiftStatusViewByShiftIdQuery(shiftId), cancellationToken);
        return currentUnapprovedShiftStatusView;
    }

    private static ShiftStatus GetShiftStatus(LineItemView[] lineItemsForReview)
    {
        return lineItemsForReview.Length > 0
        ? ShiftStatus.NotApprovedWithIssues
        : ShiftStatus.NotApprovedWithoutIssues;
    }
}