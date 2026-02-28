

using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetPlannedLineItemsByTimeRange;

internal class GetPlannedLineItemsByTimeRangeQueryHandler(
    ILineItemReadOnlyRepository lineItemReadOnlyRepository) : IRequestHandler<GetPlannedLineItemsByTimeRangeQuery, LineItemView[]>
{
    public async Task<LineItemView[]> Handle(GetPlannedLineItemsByTimeRangeQuery request, CancellationToken cancellationToken)
    {
        var lineItems = await lineItemReadOnlyRepository
            .GetPlannedLineItemsByLineCodeAndTimeRangeAsync(
                request.LineCode, request.StartDate, request.EndDate, 
                cancellationToken);
        
        return lineItems.ToArray();
    }
}