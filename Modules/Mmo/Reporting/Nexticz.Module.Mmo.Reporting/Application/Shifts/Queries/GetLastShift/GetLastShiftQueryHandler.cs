using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetLastShift;

internal class GetLastShiftQueryHandler(
    IShiftReadOnlyRepository shiftReadOnlyRepository) 
    : IRequestHandler<GetLastShiftQuery, ErrorOr<Shift>>
{
    public async Task<ErrorOr<Shift>> Handle(GetLastShiftQuery request, CancellationToken cancellationToken)
    {
        var shift = await shiftReadOnlyRepository.GetLastShiftAsync(cancellationToken);
        
        if (shift is null)
            return ShiftErrors.ShiftNotFound;
        
        return shift;
    }
}