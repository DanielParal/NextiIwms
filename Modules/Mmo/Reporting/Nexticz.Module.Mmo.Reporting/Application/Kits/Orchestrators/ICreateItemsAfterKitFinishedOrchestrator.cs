using ErrorOr;
using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Kits.Orchestrators;

internal interface ICreateItemsAfterKitFinishedOrchestrator
{
    Task<ErrorOr<Success>> OrchestrateAsync(Shift currentShift, KitWashingFinishedContract finishedKit, KitWashingFinishedContract? finishedSisterKit, CancellationToken cancellationToken);
}