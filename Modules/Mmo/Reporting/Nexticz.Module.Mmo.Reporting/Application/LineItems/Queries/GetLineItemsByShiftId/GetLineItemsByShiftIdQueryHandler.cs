using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemsByShiftId;

internal class GetLineItemsByShiftIdQueryHandler(
    ILineItemReadOnlyRepository readOnlyRepository) 
    : IRequestHandler<GetLineItemsByShiftIdQuery, LineItemView[]>
{
    public async Task<LineItemView[]> Handle(GetLineItemsByShiftIdQuery request, CancellationToken cancellationToken)
    {
        var lineItems = await readOnlyRepository.GetLineItemsByShiftIdAsync(request.ShiftId, cancellationToken);
        return lineItems.ToArray();
    }
}