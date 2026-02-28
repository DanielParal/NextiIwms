using Nexticz.Module.Mmo.Drying.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Drying.Application.Interfaces;

internal interface IKitReadOnlyRepository
{
    Task<Kit?> GetKitByCompletedKitsCountAsync(int completedKitsCount, CancellationToken cancellationToken);
}