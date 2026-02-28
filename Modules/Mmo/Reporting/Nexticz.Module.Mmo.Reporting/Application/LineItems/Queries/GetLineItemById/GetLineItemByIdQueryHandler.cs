using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemById;

internal class GetLineItemByIdQueryHandler(
    IReportingReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetLineItemByIdQuery, ErrorOr<LineItemView>>
{
    public async Task<ErrorOr<LineItemView>> Handle(GetLineItemByIdQuery request, CancellationToken cancellationToken)
    {
        var lineItem = await readOnlyEventStoreRepository.GetByIdAsync<LineItemView>(request.Id, cancellationToken);

        if (lineItem is null)
            return LineItemErrors.LineItemNotFound;
        
        return lineItem;
    }
}