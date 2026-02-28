using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Interfaces;

internal interface IShiftReadOnlyRepository
{
    Task<Shift?> GetShiftByStartDateAndEndDateAsync(DateTimeOffset startDate, DateTimeOffset endDate,
        CancellationToken cancellationToken);
    Task<Shift?> GetLastShiftAsync(CancellationToken cancellationToken);
    Task<Shift?> GetNextToLastShiftAsync(CancellationToken cancellationToken);
}