using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemsForReviewByShiftId;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Commands.UnapproveShift;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftById;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Orchestrators;

internal class UnapproveShiftOrchestrator(
    ISender sender) : IUnapproveShiftOrchestrator
{
    public async Task<ErrorOr<Success>> OrchestrateAsync(Guid shiftId, CancellationToken cancellationToken)
    {
        var shift = await sender.Send(new GetShiftByIdQuery(shiftId), cancellationToken);
        if (shift.IsError)
            return shift.Errors;

        if (!shift.Value.IsApproved)
            return Result.Success;
        
        var lineItemsForReview = await sender.Send(new GetLineItemsForReviewByShiftIdQuery(shiftId), cancellationToken);
        if (lineItemsForReview.Length == 0)
            return Result.Success;
        
        return await sender.Send(new UnapproveShiftCommand(shift.Value), cancellationToken);
    }
}