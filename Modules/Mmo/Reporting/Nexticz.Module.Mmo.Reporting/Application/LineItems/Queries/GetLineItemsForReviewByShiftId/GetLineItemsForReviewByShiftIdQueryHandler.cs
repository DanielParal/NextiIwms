using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemsByShiftId;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemsForReviewByShiftId;

internal class GetLineItemsForReviewByShiftIdQueryHandler(
    IReportingReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IRequestHandler<GetLineItemsForReviewByShiftIdQuery, LineItemView[]>
{
    public async Task<LineItemView[]> Handle(GetLineItemsForReviewByShiftIdQuery request, CancellationToken cancellationToken)
    {
        var lineItems = 
            await readOnlyEventStoreRepository.GetAllByConditionAsync<LineItemView>(
                x => x.ShiftId == request.ShiftId && x.Type == LineItemType.Downtime, cancellationToken);
        
        var lineItemsForReview = LineItemForReviewFilter.FilterLineItemsForReview(lineItems.ToArray());
        return lineItemsForReview;       
    }
}