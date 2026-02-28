using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetUnapprovedShiftStatusViews;

internal class GetUnapprovedShiftStatusViewsQueryHandler(
    IReportingReadOnlyEventStoreRepository reportingReadOnlyEventStoreRepository) : IRequestHandler<GetUnapprovedShiftStatusViewsQuery, UnapprovedShiftStatusView[]>
{
    public async Task<UnapprovedShiftStatusView[]> Handle(GetUnapprovedShiftStatusViewsQuery request, CancellationToken cancellationToken)
    {
        var unapprovedShiftStatusViews = await reportingReadOnlyEventStoreRepository.GetAllAsync<UnapprovedShiftStatusView>(cancellationToken);
        return unapprovedShiftStatusViews.ToArray();
    }
}