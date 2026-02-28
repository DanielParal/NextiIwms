using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLastItemByLineCode;

internal class GetLastItemByLineCodeQueryHandler(
    IReportingReadOnlyEventStoreRepository reportingReadOnlyEventStoreRepository) : IRequestHandler<GetLastItemByLineCodeQuery, LastItemPerLineView?>
{
    public async Task<LastItemPerLineView?> Handle(GetLastItemByLineCodeQuery request, CancellationToken cancellationToken)
    {
        var lastKitEndDatePerLine = await reportingReadOnlyEventStoreRepository.GetFirstByConditionAsync<LastItemPerLineView>(x => x.Id == request.LineCode, cancellationToken);
        return lastKitEndDatePerLine;
    }
}