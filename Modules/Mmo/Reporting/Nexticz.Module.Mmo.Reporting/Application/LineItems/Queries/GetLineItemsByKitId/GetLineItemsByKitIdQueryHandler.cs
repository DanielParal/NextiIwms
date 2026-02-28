using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemsByKitId;

internal class GetLineItemsByKitIdQueryHandler(
    ILineItemReadOnlyRepository readOnlyRepository) 
    : IRequestHandler<GetLineItemsByKitIdQuery, LineItemView[]>
{
    public async Task<LineItemView[]> Handle(GetLineItemsByKitIdQuery request, CancellationToken cancellationToken)
    {
        var lineItems = await readOnlyRepository.GetLineItemsByKitIdAsync(request.KitId, cancellationToken);
        return lineItems.ToArray();
    }
}