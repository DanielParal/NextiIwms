using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate.Events;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Commands.ChangeShiftStatus;

internal class ChangeShiftStatusCommandHandler(
    IClock clock,
    IReportingUnitOfWork unitOfWork,
    ILogger<ChangeShiftStatusCommandHandler> logger) : IRequestHandler<ChangeShiftStatusCommand, Success>
{
    public Task<Success> Handle(ChangeShiftStatusCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("[MMO] [Start] [ChangeShiftStatusCommand]");
        var shiftStatusChanged = new ShiftStatusChangedEvent(request.ShiftId, clock.UtcNowOffset, request.FromStatus, request.ToStatus);
        unitOfWork.AppendEvent(request.ShiftId, shiftStatusChanged);
        logger.LogInformation("[MMO] [End] [ChangeShiftStatusCommand]");
        return Task.FromResult(Result.Success);
    }
}