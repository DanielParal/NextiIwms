using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Interfaces;

internal interface IKitTypeReadOnlyRepository
{
    Task<KitType?> GetByCodeAsync(string code, CancellationToken cancellationToken);
}