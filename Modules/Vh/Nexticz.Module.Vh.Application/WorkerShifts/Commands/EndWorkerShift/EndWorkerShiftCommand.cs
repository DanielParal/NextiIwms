using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.WorkerShifts.Commands.EndWorkerShift;

public record EndWorkerShiftCommand(Guid Id, DateTime End) : IRequest<ErrorOr<Created>>;
