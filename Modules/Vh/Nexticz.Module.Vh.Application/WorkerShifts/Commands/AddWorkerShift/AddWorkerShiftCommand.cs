using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.WorkerShifts;

namespace Nexticz.Module.Vh.Application.WorkerShifts.Commands.AddWorkerShift;

public record AddWorkerShiftCommand(AddWorkerShiftRequest AddWorkerShiftRequest) : IRequest<ErrorOr<Created>>;