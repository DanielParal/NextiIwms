using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Interfaces;

internal interface IPackagingTypeReadOnlyRepository
{
    Task<PackagingType?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<IReadOnlyList<PackagingType>> GetPackagingTypesByCodesAsync(string[] codes, CancellationToken cancellationToken);
}