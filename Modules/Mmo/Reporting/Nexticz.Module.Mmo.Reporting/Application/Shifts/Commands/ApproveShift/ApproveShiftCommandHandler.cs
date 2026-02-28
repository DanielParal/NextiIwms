using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemsForReviewByShiftId;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftById;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate.Events;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Commands.ApproveShift;

internal class ApproveShiftCommandHandler(
    ISender sender, 
    ILogger<ApproveShiftCommandHandler> logger,
    IClock clock,
    IReportingUnitOfWork unitOfWork,
    ICurrentUserProvider currentUserProvider) : IRequestHandler<ApproveShiftCommand, ErrorOr<Shift>>
{
    public async Task<ErrorOr<Shift>> Handle(ApproveShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await sender.Send(new GetShiftByIdQuery(request.ShiftId), cancellationToken);
        if (shift.IsError)
        {
            logger.LogWarning("MMO - Reporting - Cannot approve shift. Shift does not exist. ShiftId: {ShiftId}.", request.ShiftId);
            return shift.Errors;       
        }
        
        var lineItemsForReview = await sender.Send(new GetLineItemsForReviewByShiftIdQuery(shift.Value.Id), cancellationToken);

        if (lineItemsForReview.Length > 0)
        {
            logger.LogWarning("MMO - Reporting - Cannot approve shift. There are pending items for review. ShiftId: {ShiftId}. Line items for review count: {ItemsCount}.",
                shift.Value.Id, lineItemsForReview.Length);
            return ShiftErrors.ValidationThereArePendingItemsForReview;
        }
        
        shift.Value.Approve(currentUserProvider.GetCurrentUser().UserName, clock.UtcNowOffset);
        
        var shiftApprovedEvent = new ShiftApprovedEvent(shift.Value.Id, shift.Value.ApprovedBy!, shift.Value.ApprovedAt!.Value);
        unitOfWork.AppendEvent(shift.Value.Id, shiftApprovedEvent);
        
        logger.LogInformation("MMO - Reporting - shift is approved. ShiftId: {ShiftId}.", request.ShiftId);
        
        return shift.Value;
    }
}