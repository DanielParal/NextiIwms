using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.Reports.Commands.DeleteReportPerformanceEvaluetion;

public record DeleteReportPerformanceEvaluationCommand(Guid WorkerShiftId) : IRequest<ErrorOr<Deleted>>;