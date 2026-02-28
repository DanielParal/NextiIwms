using Marten;
using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftsByIds;

internal class GetShiftsByIdsQueryHandler(
    IReportingReadOnlyEventStoreRepository reportingReadOnlyEventStoreRepository)  : IRequestHandler<GetShiftsByIdsQuery, Shift[]>
{
    public async Task<Shift[]> Handle(GetShiftsByIdsQuery request, CancellationToken cancellationToken)
    {
        var shifts =
            await reportingReadOnlyEventStoreRepository.GetAllByConditionAsync<Shift>(x => x.Id.IsOneOf(request.Ids),
                cancellationToken);
        
        return shifts.ToArray();
    }
}