using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Interfaces;

internal interface IPackagingCirculationReadOnlyRepository
{
    Task<PackagingCirculation?> GetByCodeAsync(string code, CancellationToken cancellationToken);
}