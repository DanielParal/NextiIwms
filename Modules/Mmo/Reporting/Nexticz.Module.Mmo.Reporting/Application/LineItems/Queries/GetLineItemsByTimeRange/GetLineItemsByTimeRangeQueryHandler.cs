using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemsByTimeRange;

internal class GetLineItemsByTimeRangeQueryHandler(
    ILineItemReadOnlyRepository lineItemReadOnlyRepository) : IRequestHandler<GetLineItemsByTimeRangeQuery, LineItemView[]>
{
    public async Task<LineItemView[]> Handle(GetLineItemsByTimeRangeQuery request, CancellationToken cancellationToken)
    {
        var lineItems = await lineItemReadOnlyRepository
            .GetLineItemsByTimeRangeAsync(request.StartDate, request.EndDate, cancellationToken);
        
        return lineItems.ToArray();
    }
}