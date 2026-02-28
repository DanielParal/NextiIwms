using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.Reports.Commands.CreateReportPerformanceEvaluation;

public record CreateReportPerformanceEvaluationCommand(Guid WorkerShiftId) : IRequest<ErrorOr<Created>>;