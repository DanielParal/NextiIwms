using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.ReportPerformanceEvaluations;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.ReportActivities.Common.Models;

namespace Nexticz.Module.Vh.Application.Reports.Commands.DeleteReportPerformanceEvaluetion;

public class DeleteReportPerformanceEvaluationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteReportPerformanceEvaluationCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteReportPerformanceEvaluationCommand command,
        CancellationToken cancellationToken)
    {
        var reportActivities =
            await unitOfWork.ReportPerformanceEvaluationRepository.GetReportPerformanceEvaluationAsync(
                new ReportPerformanceEvaluationFilteringParams
                {
                    Filter =
                        $"[\"{nameof(ReportPerformanceEvaluation.WorkerShiftId).ToLower()}\",\"=\", \"{command.WorkerShiftId}\"]"
                },
                cancellationToken);

        unitOfWork.RemoveRange(reportActivities.data.OfType<ReportPerformanceEvaluation>().ToList());

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Deleted;
    }
}