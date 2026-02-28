using Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Interfaces;

internal interface IDriedKitReadOnlyRepository
{
    Task<DriedKit?> GetDriedKitByIdFromDryingAsync(Guid kitIdFromDrying, CancellationToken cancellationToken);
}