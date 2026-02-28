using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.Reports.Commands.CreateReportActivities;

public record CreateReportActivitiesCommand(Guid WorkerShiftId) : IRequest<ErrorOr<Created>>;