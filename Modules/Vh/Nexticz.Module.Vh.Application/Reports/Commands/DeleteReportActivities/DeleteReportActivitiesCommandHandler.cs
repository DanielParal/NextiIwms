using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.ReportActivities;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.ReportActivities.Common.Models;

namespace Nexticz.Module.Vh.Application.Reports.Commands.DeleteReportActivities;

public class DeleteReportActivitiesCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteReportActivitiesCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteReportActivitiesCommand command,
        CancellationToken cancellationToken)
    {
        var reportActivities =
            await unitOfWork.ReportActivitiesRepository.GetReportActivitiesAsync(
                new ReportActivitiesFilteringParams
                    { Filter = $"[\"{nameof(ReportActivity.WorkerShiftId).ToLower()}\",\"=\", \"{command.WorkerShiftId}\"]" },
                cancellationToken);

        unitOfWork.RemoveRange(reportActivities.data.OfType<ReportActivity>().ToList());

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Deleted;
    }
}