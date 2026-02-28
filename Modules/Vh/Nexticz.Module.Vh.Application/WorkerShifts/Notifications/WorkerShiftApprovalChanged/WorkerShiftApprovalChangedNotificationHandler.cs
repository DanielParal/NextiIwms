using MediatR;
using Nexticz.Module.Vh.Application.Reports.Commands.CreateReportActivities;
using Nexticz.Module.Vh.Application.Reports.Commands.CreateReportPerformanceEvaluation;
using Nexticz.Module.Vh.Application.Reports.Commands.DeleteReportActivities;
using Nexticz.Module.Vh.Application.Reports.Commands.DeleteReportPerformanceEvaluetion;

namespace Nexticz.Module.Vh.Application.WorkerShifts.Notifications.WorkerShiftApprovalChanged;

public class WorkerShiftApprovalChangedNotificationHandler(ISender mediatr)
    : INotificationHandler<WorkerShiftApprovalChangedNotification>
{
    public async Task Handle(WorkerShiftApprovalChangedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Approval)
        {
            var createReportActivitiesCommand = new CreateReportActivitiesCommand(notification.Id);
            await mediatr.Send(createReportActivitiesCommand, cancellationToken);

            var createReportPerformanceEvaluationCommand =
                new CreateReportPerformanceEvaluationCommand(notification.Id);
            await mediatr.Send(createReportPerformanceEvaluationCommand, cancellationToken);

            return;
        }

        var deleteReportActivitiesCommand = new DeleteReportActivitiesCommand(notification.Id);
        await mediatr.Send(deleteReportActivitiesCommand, cancellationToken);

        var deleteReportPerformanceEvaluationCommand = new DeleteReportPerformanceEvaluationCommand(notification.Id);
        await mediatr.Send(deleteReportPerformanceEvaluationCommand, cancellationToken);
    }
}