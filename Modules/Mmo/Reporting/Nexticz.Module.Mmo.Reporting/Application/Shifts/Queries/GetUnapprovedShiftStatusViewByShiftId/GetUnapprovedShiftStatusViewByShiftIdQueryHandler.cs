using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetUnapprovedShiftStatusViews;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetUnapprovedShiftStatusViewByShiftId;

internal class GetUnapprovedShiftStatusViewByShiftIdQueryHandler(
    IReportingReadOnlyEventStoreRepository reportingReadOnlyEventStoreRepository) : IRequestHandler<GetUnapprovedShiftStatusViewByShiftIdQuery, UnapprovedShiftStatusView?>
{
    public async Task<UnapprovedShiftStatusView?> Handle(GetUnapprovedShiftStatusViewByShiftIdQuery request, CancellationToken cancellationToken)
    {
        var unapprovedShiftStatusView = await reportingReadOnlyEventStoreRepository.GetFirstByConditionAsync<UnapprovedShiftStatusView>(x => x.ShiftId == request.ShiftId, cancellationToken);
        return unapprovedShiftStatusView;
    }
}