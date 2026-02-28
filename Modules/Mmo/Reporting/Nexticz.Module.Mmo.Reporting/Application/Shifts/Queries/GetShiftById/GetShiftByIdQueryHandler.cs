using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftById;

internal class GetShiftByIdQueryHandler(
    IReportingReadOnlyEventStoreRepository reportingReadOnlyEventStoreRepository) 
    : IRequestHandler<GetShiftByIdQuery, ErrorOr<Shift>>
{
    public async Task<ErrorOr<Shift>> Handle(GetShiftByIdQuery request, CancellationToken cancellationToken)
    {
        var shift = await reportingReadOnlyEventStoreRepository.GetByIdAsync<Shift>(request.Id, cancellationToken);
        
        if (shift is null)
            return ShiftErrors.ShiftNotFound;
        
        return shift;
    }
}