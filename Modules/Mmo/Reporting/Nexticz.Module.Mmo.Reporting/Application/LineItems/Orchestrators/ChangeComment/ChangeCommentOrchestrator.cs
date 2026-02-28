using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Commands.ChangeComment;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Notifications;
using Nexticz.Module.Mmo.Reporting.Application.NotificationCollectors;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Orchestrators.ChangeComment;

internal class ChangeCommentOrchestrator(
    ISender sender,
    IReportingUnitOfWork unitOfWork,
    IReportingNotificationCollector notificationCollector) : IChangeCommentOrchestrator
{
    public async Task<ErrorOr<Success>> ChangeCommentAsync(Guid id, string comment, CancellationToken cancellationToken)
    {
        unitOfWork.BeginTransaction();
        
        var result = await sender.Send(new ChangeCommentCommand(id, comment), cancellationToken);
        if (result.IsError)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            return result.Errors;
        }
        
        await unitOfWork.CommitTransactionAsync(cancellationToken);
        
        notificationCollector.AddNotification(new CommentChangedNotification(result.Value.Id, result.Value.ShiftId, comment));
        await notificationCollector.PublishNotificationsAsync(cancellationToken);
        
        return Result.Success;
    }
}