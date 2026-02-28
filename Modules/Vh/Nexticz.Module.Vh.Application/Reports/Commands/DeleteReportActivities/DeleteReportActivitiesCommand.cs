using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.Reports.Commands.DeleteReportActivities;

public record DeleteReportActivitiesCommand(Guid WorkerShiftId) : IRequest<ErrorOr<Deleted>>;