using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Orchestrators;

internal interface IShiftCreationOrchestrator
{
    Task<Shift> EnsureCurrentShiftAsync(CancellationToken cancellationToken);
}