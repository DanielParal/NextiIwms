using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.WorkerShifts;

namespace Nexticz.Module.Vh.Application.WorkerShifts.Commands.AddWorkerShiftActivity;

public record AddWorkerShiftActivityCommand(AddWorkerShiftActivityRequest AddWorkerShiftActivityRequest, Guid Id)
    : IRequest<ErrorOr<Created>>;