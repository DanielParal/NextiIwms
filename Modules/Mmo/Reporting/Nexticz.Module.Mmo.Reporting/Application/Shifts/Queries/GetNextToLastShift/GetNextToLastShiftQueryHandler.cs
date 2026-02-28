using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetNextToLastShift;

internal class GetNextToLastShiftQueryHandler(
    IShiftReadOnlyRepository shiftReadOnlyRepository) 
    : IRequestHandler<GetNextToLastShiftQuery, ErrorOr<Shift>>
{
    public async Task<ErrorOr<Shift>> Handle(GetNextToLastShiftQuery request, CancellationToken cancellationToken)
    {
        var shift = await shiftReadOnlyRepository.GetNextToLastShiftAsync(cancellationToken);
        
        if (shift is null)
            return ShiftErrors.ShiftNotFound;
        
        return shift;
    }
}