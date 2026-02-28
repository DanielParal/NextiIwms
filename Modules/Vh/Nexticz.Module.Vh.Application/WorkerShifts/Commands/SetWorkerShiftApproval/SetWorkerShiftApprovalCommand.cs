using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.WorkerShifts;

namespace Nexticz.Module.Vh.Application.WorkerShifts.Commands.SetWorkerShiftApproval;

public record SetWorkerShiftApprovalCommand(Guid Id, SetWorkerShiftApprovalRequest SetWorkerShiftApprovalRequest)
    : IRequest<ErrorOr<Created>>;