using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate.Events;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Commands.UnapproveShift;

internal class UnapproveShiftCommandHandler(
    ISender sender,
    IClock clock,
    ILogger<UnapproveShiftCommandHandler> logger,
    IReportingUnitOfWork unitOfWork) : IRequestHandler<UnapproveShiftCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UnapproveShiftCommand request, CancellationToken cancellationToken)
    {
        if (!request.Shift.IsApproved)
            return Result.Success;
        
        var unapprovedEvent = new ShiftUnapprovedEvent(request.Shift.Id, clock.UtcNowOffset);
        unitOfWork.AppendEvent(request.Shift.Id, unapprovedEvent);
        
        logger.LogInformation("MMO - Reporting - shift is unapproved. ShiftId: {ShiftId}.", request.Shift.Id);
        
        return Result.Success;
    }
}