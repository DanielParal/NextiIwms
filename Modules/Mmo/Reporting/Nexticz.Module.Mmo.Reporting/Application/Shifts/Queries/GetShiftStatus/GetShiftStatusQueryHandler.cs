using MediatR;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftStatus;

internal class GetShiftStatusQueryHandler : IRequestHandler<GetShiftStatusQuery, ShiftStatus>
{
    public Task<ShiftStatus> Handle(GetShiftStatusQuery request, CancellationToken cancellationToken)
    {
        if (request.Shift.ApprovedBy is not null)
            return Task.FromResult(ShiftStatus.Approved);
        
        return Task.FromResult(
            request.UnapprovedShiftStatusViews.FirstOrDefault(x => x.ShiftId == request.Shift.Id)?.Status ?? 
            ShiftStatus.NotApprovedWithIssues);
    }
}