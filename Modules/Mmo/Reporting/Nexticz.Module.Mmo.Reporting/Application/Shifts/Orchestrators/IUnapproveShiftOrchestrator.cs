using ErrorOr;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Orchestrators;

internal interface IUnapproveShiftOrchestrator
{
    Task<ErrorOr<Success>> OrchestrateAsync(Guid shiftId, CancellationToken cancellationToken);
}